using System.Text.Json;
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.Desktop.Models;
namespace RadiologyCenter.Desktop.Services;

public sealed class ApiException(int statusCode, string message, ApiError? error = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public ApiError? Error { get; } = error;
}

// Existing feature services keep their interface while calls execute directly in this process.
public sealed class ApiClient(BusinessRuntimeService business, TokenStorage tokens,
    AppAuthenticationStateProvider auth, AppLocalizer localizer)
{
    private static readonly SemaphoreSlim RefreshGate = new(1,1);
    public Task<T> GetAsync<T>(string path, CancellationToken ct = default) => SendAsync<T>("GET",path,null,ct);
    public Task<T> PostAsync<T>(string path, object? body = null, CancellationToken ct = default) => SendAsync<T>("POST",path,body,ct);
    public Task<T> PutAsync<T>(string path, object body, CancellationToken ct = default) => SendAsync<T>("PUT",path,body,ct);
    public Task SendAsync(string path, object? body = null, CancellationToken ct = default) => SendAsync<object>("POST",path,body,ct);
    public Task SendDeleteAsync(string path, CancellationToken ct = default) => SendAsync<object>("DELETE",path,null,ct);
    public async Task<byte[]> GetBytesAsync(string path, CancellationToken ct = default) => File(await DispatchAsync("GET",path,null,ct));
    public async Task<byte[]> PostBytesAsync(string path, object? body = null, CancellationToken ct = default) => File(await DispatchAsync("POST",path,body,ct));
    public async Task<T> PostFormAsync<T>(string path, IReadOnlyDictionary<string,string>? fields = null,
        (string Name,string FileName,string ContentType,Stream Stream)? file = null, CancellationToken ct = default)
    {
        await business.EnsureDevelopmentAsync(ct);
        ClinicUpload? upload = null;
        if (file is {} f) {
            using var memory = new MemoryStream();
            var buffer = new byte[81920]; int count;
            while ((count=await f.Stream.ReadAsync(buffer,ct))>0) {
                if (memory.Length+count>20*1024*1024) throw new ApiException(400,"Documents must be no larger than 20 MB.");
                await memory.WriteAsync(buffer.AsMemory(0,count),ct);
            }
            upload = new(f.Name,f.FileName,f.ContentType,memory.ToArray());
        }
        return Unwrap<T>(await DispatchRequestAsync(new("POST",path,
            Fields: fields is null ? null : new Dictionary<string,string>(fields,StringComparer.OrdinalIgnoreCase),
            Upload:upload,AccessToken:tokens.GetTokens()?.AccessToken,Culture:localizer.CurrentCulture),ct));
    }
    private async Task<T> SendAsync<T>(string kind,string path,object? body,CancellationToken ct) => Unwrap<T>(await DispatchAsync(kind,path,body,ct));
    private async Task<ClinicResult> DispatchAsync(string kind,string path,object? body,CancellationToken ct)
    {
        await business.EnsureDevelopmentAsync(ct);
        var request = new ClinicRequest(kind,path,body is null ? null : JsonSerializer.Serialize(body,ClinicDispatcher.JsonOptions),
            AccessToken:tokens.GetTokens()?.AccessToken,Culture:localizer.CurrentCulture);
        return await DispatchRequestAsync(request,ct);
    }
    private async Task<ClinicResult> DispatchRequestAsync(ClinicRequest request,CancellationToken ct)
    {
        if (!business.IsDevelopment && (tokens.GetTokens() is not { } session || session.ExpiresAt <= DateTime.UtcNow))
        {
            await business.SignOutAsync();
            await auth.SignOutAsync();
            throw new ApiException(401, "Your clinic session expired. Sign in again.");
        }
        var result = await business.Runtime.Dispatcher.DispatchAsync(request,ct);
        if (result.StatusCode==401 && business.IsDevelopment && tokens.GetTokens() is {} current && !request.Operation.TrimStart('/').StartsWith("api/auth/") && !request.Operation.TrimStart('/').StartsWith("auth/"))
        {
            await RefreshGate.WaitAsync(ct);
            try {
                var latest=tokens.GetTokens();
                if (latest is not null && latest.AccessToken==current.AccessToken) {
                    var refreshed=await business.Runtime.Dispatcher.DispatchAsync(new("POST","auth/refresh",
                        JsonSerializer.Serialize(new { token=latest.RefreshToken }),Culture:localizer.CurrentCulture),ct);
                    if (refreshed.IsSuccess) await auth.SignInAsync(AuthTokens.From(Unwrap<TokenResult>(refreshed),latest.Username));
                    else { await business.SignOutAsync(); await auth.SignOutAsync(); }
                }
                result=await business.Runtime.Dispatcher.DispatchAsync(request with { AccessToken=tokens.GetTokens()?.AccessToken },ct);
            } finally { RefreshGate.Release(); }
        }
        if(result.StatusCode==401 && !business.IsDevelopment) { await business.SignOutAsync(); await auth.SignOutAsync(); }
        return result;
    }
    private static byte[] File(ClinicResult result) {
        if (result.IsSuccess && result.Value is ClinicFile file) return file.Content;
        _=Unwrap<object>(result); throw new ApiException(500,"The operation did not return a file.");
    }
    internal static T Unwrap<T>(ClinicResult result) {
        var json=JsonSerializer.Serialize(result.Value,ClinicDispatcher.JsonOptions);
        var envelope=JsonSerializer.Deserialize<ApiEnvelope<T>>(json,ClinicDispatcher.JsonOptions);
        if (!result.IsSuccess || envelope is not { Success:true })
            throw new ApiException(result.StatusCode,envelope?.Error?.Message ?? envelope?.Message ?? "The operation failed.",envelope?.Error);
        return envelope.Data ?? default!;
    }
}
