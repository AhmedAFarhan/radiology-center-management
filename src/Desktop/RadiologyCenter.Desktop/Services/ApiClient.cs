using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class ApiException : Exception
{
    public int StatusCode { get; }
    public ApiError? Error { get; }

    public ApiException(int statusCode, string message, ApiError? error = null)
        : base(message)
    {
        StatusCode = statusCode;
        Error = error;
    }
}

public sealed class ApiClient
{
    public const string BaseUrl = "http://localhost:5224";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient _http;
    private readonly TokenStorage _tokenStorage;
    private readonly AppAuthenticationStateProvider _authState;

    public ApiClient(TokenStorage tokenStorage, AppAuthenticationStateProvider authState)
    {
        _tokenStorage = tokenStorage;
        _authState = authState;
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl), Timeout = TimeSpan.FromSeconds(30) };
    }

    public Task<T> GetAsync<T>(string path, CancellationToken ct = default)
        => SendCoreAsync<T>(() => new HttpRequestMessage(HttpMethod.Get, path), ct);

    public Task<T> PostAsync<T>(string path, object? body = null, CancellationToken ct = default)
        => SendCoreAsync<T>(() => new HttpRequestMessage(HttpMethod.Post, path) { Content = CreateJson(body) }, ct);

    public Task<T> PutAsync<T>(string path, object body, CancellationToken ct = default)
        => SendCoreAsync<T>(() => new HttpRequestMessage(HttpMethod.Put, path) { Content = CreateJson(body) }, ct);

    public Task SendAsync(string path, object? body = null, CancellationToken ct = default)
        => SendCoreAsync<object>(() => new HttpRequestMessage(HttpMethod.Post, path) { Content = CreateJson(body) }, ct);

    public Task SendDeleteAsync(string path, CancellationToken ct = default)
        => SendCoreAsync<object>(() => new HttpRequestMessage(HttpMethod.Delete, path), ct);

    public Task<T> PostFormAsync<T>(
        string path,
        IReadOnlyDictionary<string, string>? fields = null,
        (string Name, string FileName, string ContentType, Stream Stream)? file = null,
        CancellationToken ct = default)
        => SendCoreAsync<T>(() =>
        {
            var content = new MultipartFormDataContent();
            if (fields is not null)
            {
                foreach (var (name, value) in fields)
                {
                    if (!string.IsNullOrEmpty(value))
                        content.Add(new StringContent(value!), name);
                }
            }

            if (file is { } f)
            {
                var streamContent = new StreamContent(f.Stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(f.ContentType);
                content.Add(streamContent, "file", f.FileName);
            }

            return new HttpRequestMessage(HttpMethod.Post, path) { Content = content };
        }, ct);

    public Task<byte[]> GetBytesAsync(string path, CancellationToken ct = default)
        => SendCoreRawAsync(() => new HttpRequestMessage(HttpMethod.Get, path), ct);

    private async Task<T> SendCoreAsync<T>(Func<HttpRequestMessage> requestFactory, CancellationToken ct)
    {
        var request = requestFactory();
        var tokens = _tokenStorage.GetTokens();
        if (tokens is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized && tokens is not null)
        {
            var refreshed = await TryRefreshAsync(tokens, ct);
            if (refreshed is null)
            {
                await _authState.SignOutAsync();
            }
            else
            {
                request = requestFactory();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
                response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            }
        }

        return await DeserializeAsync<T>(response, ct);
    }

    private async Task<byte[]> SendCoreRawAsync(Func<HttpRequestMessage> requestFactory, CancellationToken ct)
    {
        var request = requestFactory();
        var tokens = _tokenStorage.GetTokens();
        if (tokens is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized && tokens is not null)
        {
            var refreshed = await TryRefreshAsync(tokens, ct);
            if (refreshed is null)
            {
                await _authState.SignOutAsync();
            }
            else
            {
                request = requestFactory();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
                response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            }
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiException(401, "Your session has expired. Please sign in again.");

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            ApiError? error = null;
            try
            {
                var envelope = JsonSerializer.Deserialize<ApiEnvelope>(content, JsonOptions);
                error = envelope?.Error;
                if (string.IsNullOrWhiteSpace(error?.Message))
                    error = new ApiError { Message = envelope?.Message ?? "Request failed." };
            }
            catch
            {
                // non-JSON error body
            }

            throw new ApiException((int)response.StatusCode, error?.Message ?? $"Request failed ({(int)response.StatusCode}).", error);
        }

        return await response.Content.ReadAsByteArrayAsync(ct);
    }

    private async Task<TokenResult?> TryRefreshAsync(AuthTokens tokens, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh")
            {
                Content = CreateJson(new { token = tokens.RefreshToken }),
            };

            var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!response.IsSuccessStatusCode)
                return null;

            var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<TokenResult>>(JsonOptions, ct);
            if (envelope is not { Success: true } || envelope.Data is null)
                return null;

            await _authState.SignInAsync(new AuthTokens(
                envelope.Data.AccessToken,
                envelope.Data.RefreshToken,
                envelope.Data.ExpiresAt,
                envelope.Data.RefreshTokenExpiresAt,
                tokens.Username,
                envelope.Data.MustChangePassword));

            return envelope.Data;
        }
        catch
        {
            return null;
        }
    }

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            ApiError? error = null;
            try
            {
                var fail = JsonSerializer.Deserialize<ApiEnvelope>(content, JsonOptions);
                error = fail?.Error;
                if (string.IsNullOrWhiteSpace(error?.Message))
                    error = new ApiError { Message = fail?.Message ?? "Request failed." };
            }
            catch
            {
                // non-JSON error body
            }

            throw new ApiException((int)response.StatusCode, error?.Message ?? $"Request failed ({(int)response.StatusCode}).", error);
        }

        try
        {
            var envelope = JsonSerializer.Deserialize<ApiEnvelope<T>>(content, JsonOptions);
            if (envelope is null || !envelope.Success)
                throw new ApiException((int)response.StatusCode, envelope?.Error?.Message ?? envelope?.Message ?? "Request failed.", envelope?.Error);

            return envelope.Data ?? default!;
        }
        catch (JsonException)
        {
            throw new ApiException((int)response.StatusCode, "Invalid response from server.");
        }
    }

    private static HttpContent? CreateJson(object? body)
    {
        if (body is null)
            return null;
        return JsonContent.Create(body, options: JsonOptions);
    }
}
