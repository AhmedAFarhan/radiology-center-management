using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
namespace RadiologyCenter.Desktop.Services;

/// <summary>Connects to the one clinic image service. Workstations never start their own archive.</summary>
public sealed class PacsService : IDisposable
{
    public sealed record PacsStudy(string StudyInstanceUid,string PatientId,string? PatientName,string? StudyDate,string? Modality,string? AccessionNumber);
    public static PacsService? Instance { get; private set; }
    private readonly HttpClient _http;
    private readonly BusinessRuntimeService _business;
    public string HttpEndpoint => DesktopConfiguration.Profile?.PacsUrl ?? (DesktopConfiguration.IsDevelopment ? "http://127.0.0.1:18042" : "");
    public string ViewerBaseUrl => HttpEndpoint + "/ohif";
    public bool IsReady => !string.IsNullOrEmpty(HttpEndpoint) && _business.PacsAuthorization is not null;
    public PacsService(IHttpClientFactory clients,IServiceScopeFactory scopes)
    {
        Instance=this; _http=clients.CreateClient("PacsDesktop"); _http.Timeout=TimeSpan.FromSeconds(15);
        using var scope=scopes.CreateScope(); _business=scope.ServiceProvider.GetRequiredService<BusinessRuntimeService>();
    }
    public async Task<IReadOnlyList<PacsStudy>> GetStudiesAsync(string? patientId=null,CancellationToken ct=default)
    {
        if (!IsReady) throw new InvalidOperationException("Configure your image server account before opening studies.");
        using var request=new HttpRequestMessage(HttpMethod.Get,HttpEndpoint+"/dicom-web/studies"+
            (string.IsNullOrWhiteSpace(patientId)? "" : "?PatientID="+Uri.EscapeDataString(patientId.Trim())));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/dicom+json"));
        if (_business.PacsAuthorization is {} authorization) request.Headers.TryAddWithoutValidation("Authorization",authorization);
        using var response=await _http.SendAsync(request,ct); response.EnsureSuccessStatusCode();
        using var json=await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct),cancellationToken:ct);
        var studies=new List<PacsStudy>();
        foreach (var item in json.RootElement.EnumerateArray())
            if (Tag(item,"0020000D") is {} uid) studies.Add(new(uid,Tag(item,"00100020")??"",Person(item),Tag(item,"00080020"),Tag(item,"00080061")??Tag(item,"00080060"),Tag(item,"00080050")));
        return studies;
    }
    public async Task VerifyAccountAsync(string userName,string password,CancellationToken ct=default)
    {
        if(string.IsNullOrWhiteSpace(HttpEndpoint)) throw new InvalidOperationException("Configure the clinic image server address in clinic settings first.");
        if(string.IsNullOrWhiteSpace(userName) || userName.Contains(':') || string.IsNullOrEmpty(password))
            throw new InvalidOperationException("Enter your image server username and password.");
        using var request=new HttpRequestMessage(HttpMethod.Get,HttpEndpoint+"/dicom-web/studies?limit=1");
        request.Headers.Authorization=new AuthenticationHeaderValue("Basic",Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userName+":"+password)));
        using var response=await _http.SendAsync(request,ct);
        if(response.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden)
            throw new InvalidOperationException("The image server account is invalid or does not allow viewing images.");
        response.EnsureSuccessStatusCode();
    }
    private static string? Tag(JsonElement item,string tag) => item.TryGetProperty(tag,out var el) && el.TryGetProperty("Value",out var value)
        && value.GetArrayLength()>0 ? value[0].ToString() : null;
    private static string? Person(JsonElement item) => item.TryGetProperty("00100010",out var el) && el.TryGetProperty("Value",out var v)
        && v.GetArrayLength()>0 && v[0].TryGetProperty("Alphabetic",out var n) ? n.GetString() : null;
    public void Dispose() => _http.Dispose();
}
