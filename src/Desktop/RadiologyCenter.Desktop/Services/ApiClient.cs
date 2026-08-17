using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MudBlazor;
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
    private readonly AppLocalizer _localizer;
    private readonly ISnackbar _snackbar;
    private Task<TokenResult?>? _refreshTask;

    public ApiClient(TokenStorage tokenStorage, AppAuthenticationStateProvider authState, AppLocalizer localizer, ISnackbar snackbar)
    {
        _tokenStorage = tokenStorage;
        _authState = authState;
        _localizer = localizer;
        _snackbar = snackbar;
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
        var response = await SendWithRefreshAsync(requestFactory, ct);
        return await DeserializeAsync<T>(response, ct);
    }

    private async Task<byte[]> SendCoreRawAsync(Func<HttpRequestMessage> requestFactory, CancellationToken ct)
    {
        var response = await SendWithRefreshAsync(requestFactory, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiException(401, _localizer.Error.SignOut);

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

    private async Task<HttpResponseMessage> SendWithRefreshAsync(Func<HttpRequestMessage> requestFactory, CancellationToken ct)
    {
        var request = requestFactory();
        ApplyAuth(request, null);
        var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        var tokens = _tokenStorage.GetTokens();
        if (tokens is null)
            return response;

        var refreshed = await GetOrStartRefreshAsync(tokens, ct);
        if (refreshed is null)
        {
            await _authState.SignOutAsync();
            _snackbar.Add(_localizer.Error.SignOut, Severity.Warning);
            return response;
        }

        request = requestFactory();
        ApplyAuth(request, refreshed.AccessToken);
        return await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
    }

    private void ApplyAuth(HttpRequestMessage request, string? accessToken)
    {
        request.Headers.AcceptLanguage.TryParseAdd(_localizer.CurrentCulture);
        if (accessToken is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return;
        }

        var tokens = _tokenStorage.GetTokens();
        if (tokens is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
    }

    private Task<TokenResult?> GetOrStartRefreshAsync(AuthTokens tokens, CancellationToken ct)
    {
        if (_refreshTask is not null)
            return _refreshTask;

        _refreshTask = RefreshCoreAsync(tokens, ct);
        _ = _refreshTask.ContinueWith(
            _ => _refreshTask = null,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        return _refreshTask;
    }

    private async Task<TokenResult?> RefreshCoreAsync(AuthTokens tokens, CancellationToken ct)
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
