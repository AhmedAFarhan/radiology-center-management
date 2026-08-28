using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace RadiologyCenter.Desktop.Services;

public sealed class PacsService : LocalProcessServiceBase, IDisposable
{
    public const int HttpPort = 18042;
    public const int DicomPort = 14242;
    private const string DicomAet = "EGCAREPACS";

    public sealed record PacsStudy(
        string StudyInstanceUid,
        string PatientId,
        string? PatientName,
        string? StudyDate,
        string? Modality,
        string? AccessionNumber);

    private static readonly string[] PluginFiles =
    {
        "OrthancDicomWeb.dll",
        "libOrthancOHIF-Windows64.dll",
        "OrthancWebViewer.dll",
    };

    private static readonly string[] RuntimeFiles =
    {
        "iconv.dll", "libcairo-2.dll", "libffi-6.dll", "libgdk_pixbuf-2.0-0.dll",
        "libgio-2.0-0.dll", "libglib-2.0-0.dll", "libgmodule-2.0-0.dll",
        "libgobject-2.0-0.dll", "libgthread-2.0-0.dll", "libintl-8.dll",
        "libjpeg-62.dll", "libopenjp2.dll", "libopenslide-0.dll",
        "libpixman-1-0.dll", "libpng16-16.dll", "libsqlite3-0.dll",
        "libtiff-5.dll", "libxml2-2.dll", "zlib1.dll",
    };

    public static PacsService? Instance { get; private set; }

    public string HttpEndpoint => $"http://127.0.0.1:{HttpPort}";
    public string ViewerBaseUrl => $"{HttpEndpoint}/ohif";

    protected override string StartFailureMessage
        => $"PACS (Orthanc) failed to start within {StartupTimeoutMs}ms.";

    protected override string HealthCheckUrl
        => $"{HttpEndpoint}/system";

    protected override IReadOnlyList<int> Ports => new[] { HttpPort, DicomPort };

    protected override int StartupTimeoutMs => 20000;

    protected override int HealthCheckIntervalMs => 500;

    public PacsService(IHttpClientFactory? httpClientFactory = null)
        : base(httpClientFactory)
    {
        Instance = this;
    }

    public async Task<IReadOnlyList<PacsStudy>> GetStudiesAsync(CancellationToken ct = default)
        => await GetStudiesAsync(null, ct);

    public async Task<IReadOnlyList<PacsStudy>> GetStudiesAsync(string? patientId, CancellationToken ct = default)
    {
        using var httpClient = HttpClientFactory?.CreateClient() ?? new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        var url = $"{HttpEndpoint}/dicom-web/studies";
        if (!string.IsNullOrWhiteSpace(patientId))
            url += $"?PatientID={Uri.EscapeDataString(patientId.Trim())}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("Accept", "application/dicom+json");

        using var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        var studies = new List<PacsStudy>();
        if (document.RootElement.ValueKind != JsonValueKind.Array)
            return studies;

        foreach (var item in document.RootElement.EnumerateArray())
        {
            var uid = GetString(item, "0020000D");
            if (string.IsNullOrWhiteSpace(uid))
                continue;

            studies.Add(new PacsStudy(
                StudyInstanceUid: uid,
                PatientId: GetString(item, "00100020"),
                PatientName: GetPersonName(item, "00100010"),
                StudyDate: GetString(item, "00080020"),
                Modality: GetString(item, "00080060"),
                AccessionNumber: GetString(item, "00080050")));
        }

        return studies;
    }

    private static string? GetString(JsonElement item, string tag)
    {
        if (!item.TryGetProperty(tag, out var el) || el.ValueKind != JsonValueKind.Object)
            return null;
        if (!el.TryGetProperty("Value", out var value) || value.ValueKind != JsonValueKind.Array)
            return null;
        return value.GetArrayLength() == 0 ? null : value[0].ToString();
    }

    private static string? GetPersonName(JsonElement item, string tag)
    {
        if (!item.TryGetProperty(tag, out var el) || el.ValueKind != JsonValueKind.Object)
            return null;
        if (!el.TryGetProperty("Value", out var value) || value.ValueKind != JsonValueKind.Array)
            return null;
        if (value.GetArrayLength() == 0 || value[0].ValueKind != JsonValueKind.Object)
            return null;
        return value[0].TryGetProperty("Alphabetic", out var alpha) ? alpha.ToString() : null;
    }

    private static string DataRoot =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EGcare", "pacs");

    protected override Process CreateProcess()
    {
        var (root, exePath) = ResolveLocalCopy();

        var dataDir = Path.Combine(DataRoot, "data");
        Directory.CreateDirectory(dataDir);
        var configPath = WriteConfig(root, dataDir);

        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{configPath}\"",
                WorkingDirectory = root,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            },
        };
    }

    protected override bool IsOwnProcess(Process process)
    {
        try
        {
            if (process.ProcessName.Contains("Orthanc", StringComparison.OrdinalIgnoreCase))
                return true;

            var path = process.MainModule?.FileName;
            return path is not null && path.Contains("Orthanc", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private (string root, string exePath) ResolveLocalCopy()
    {
        var orthancDir = Path.Combine(DataRoot, "orthanc");
        var exePath = Path.Combine(orthancDir, "Orthanc.exe");

        if (!File.Exists(exePath))
        {
            var source = ResolveSourceDir();
            Directory.CreateDirectory(orthancDir);

            CopyFile(source, orthancDir, "Orthanc.exe");

            var localPlugins = Path.Combine(orthancDir, "plugins");
            Directory.CreateDirectory(localPlugins);

            foreach (var plugin in PluginFiles)
                CopyFromAnywhere(source, localPlugins, plugin);

            foreach (var runtime in RuntimeFiles)
                CopyFromAnywhere(source, localPlugins, runtime);
        }

        return (orthancDir, exePath);
    }

    private static string ResolveSourceDir()
    {
        var bundled = Path.Combine(AppContext.BaseDirectory, "pacs");
        if (File.Exists(Path.Combine(bundled, "Orthanc.exe")))
            return bundled;

        var fromEnv = Environment.GetEnvironmentVariable("EGCARE_ORTHANC_DIR");
        if (!string.IsNullOrWhiteSpace(fromEnv) && File.Exists(Path.Combine(fromEnv, "Orthanc.exe")))
            return fromEnv;

        var installed = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Orthanc Server");
        if (File.Exists(Path.Combine(installed, "Orthanc.exe")))
            return installed;

        throw new InvalidOperationException(
            "Orthanc.exe not found. Bundle it under 'pacs' in the app directory, set EGCARE_ORTHANC_DIR, " +
            "or install Orthanc Server.");
    }

    private static void CopyFile(string sourceDir, string destDir, string fileName)
    {
        var src = Path.Combine(sourceDir, fileName);
        if (!File.Exists(src))
            throw new FileNotFoundException($"Missing Orthanc file: {src}", src);
        File.Copy(src, Path.Combine(destDir, fileName), overwrite: true);
    }

    private static void CopyFromAnywhere(string sourceDir, string destDir, string fileName)
    {
        foreach (var candidate in new[] { "Plugins", "plugins", "Tools" })
        {
            var src = Path.Combine(sourceDir, candidate, fileName);
            if (File.Exists(src))
            {
                File.Copy(src, Path.Combine(destDir, fileName), overwrite: true);
                return;
            }
        }
    }

    private static string WriteConfig(string orthancDir, string dataDir)
    {
        var configPath = Path.Combine(DataRoot, "orthanc.json");

        var plugins = PluginFiles
            .Select(p => Path.Combine(orthancDir, "plugins", p))
            .ToArray();

        var config = new Dictionary<string, object?>
        {
            ["Name"] = "EGcare PACS",
            ["StorageDirectory"] = dataDir,
            ["IndexDirectory"] = dataDir,
            ["DicomPort"] = DicomPort,
            ["HttpPort"] = HttpPort,
            ["DicomAet"] = DicomAet,
            ["AuthenticationEnabled"] = false,
            ["RemoteAccessAllowed"] = true,
            ["HttpsPort"] = 0,
            ["Plugins"] = plugins,
        };

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configPath, json);
        return configPath;
    }
}