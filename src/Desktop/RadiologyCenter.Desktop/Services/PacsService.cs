using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RadiologyCenter.Desktop.Services;

public sealed class PacsService : IDisposable
{
    public const int HttpPort = 18042;
    public const int DicomPort = 14242;
    private const string DicomAet = "EGCAREPACS";
    private const int StartupTimeoutMs = 20000;
    private const int HealthCheckIntervalMs = 500;

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
    public bool IsReady => _startupTask is { IsCompletedSuccessfully: true };
    public string? StartupError { get; private set; }

    public async Task<IReadOnlyList<PacsStudy>> GetStudiesAsync(CancellationToken ct = default)
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{HttpEndpoint}/dicom-web/studies");
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

    private Process? _process;
    private Task? _startupTask;

    public PacsService()
    {
        Instance = this;
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Dispose();
    }

    public Task StartAsync()
    {
        if (_startupTask is not null && !_startupTask.IsCanceled)
            return _startupTask;

        _startupTask = StartCoreAsync();
        return _startupTask;
    }

    public async Task RetryAsync()
    {
        KillProcess();
        _startupTask = null;
        await StartAsync();
    }

    private async Task StartCoreAsync()
    {
        try
        {
            var (root, exePath) = ResolveLocalCopy();
            EnsurePortIsFree();

            var dataDir = Path.Combine(DataRoot, "data");
            Directory.CreateDirectory(dataDir);
            var configPath = WriteConfig(root, dataDir);

            var stderr = new StringWriter();

            _process = new Process
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
                EnableRaisingEvents = true,
            };

            _process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                    stderr.WriteLine(e.Data);
            };

            _process.Start();
            _process.BeginErrorReadLine();

            var started = await WaitForStartupAsync();
            if (!started)
            {
                KillProcess();
                var error = stderr.ToString();
                throw new InvalidOperationException(
                    $"PACS (Orthanc) failed to start within {StartupTimeoutMs}ms.\nError output:\n{error}");
            }
        }
        catch (Exception ex)
        {
            StartupError = ex.Message;
            throw;
        }
    }

    public void Stop()
    {
        KillProcess();
        EnsurePortIsFree();
    }

    public void Dispose() => Stop();

    private async Task<bool> WaitForStartupAsync()
    {
        var elapsed = 0;
        while (elapsed < StartupTimeoutMs)
        {
            if (_process is null || _process.HasExited)
                return false;

            try
            {
                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
                var response = await httpClient.GetAsync($"{HttpEndpoint}/system");
                if (response.IsSuccessStatusCode)
                    return true;
            }
            catch
            {
                // not ready yet
            }

            await Task.Delay(HealthCheckIntervalMs);
            elapsed += HealthCheckIntervalMs;
        }
        return false;
    }

    private void KillProcess()
    {
        if (_process is { HasExited: false })
        {
            try { _process.Kill(entireProcessTree: true); _process.WaitForExit(5000); }
            catch { /* already gone */ }
            finally { _process.Dispose(); _process = null; }
        }
    }

    private static void EnsurePortIsFree()
    {
        try
        {
            using var proc = Process.Start(new ProcessStartInfo
            {
                FileName = "netstat.exe",
                Arguments = "-ano",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            });

            if (proc is null) return;
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit(2000);

            foreach (var port in new[] { HttpPort, DicomPort })
            {
                foreach (Match match in Regex.Matches(output, $":{port}\\s+\\S+\\s+LISTENING\\s+(\\d+)"))
                {
                    var pid = int.Parse(match.Groups[1].Value);
                    if (pid <= 0) continue;

                    try
                    {
                        using var victim = Process.GetProcessById(pid);
                        if (!IsOurOrthanc(victim))
                            continue;
                        victim.Kill(entireProcessTree: true);
                        victim.WaitForExit(5000);
                    }
                    catch
                    {
                        // already gone or access denied
                    }
                }
            }
        }
        catch { /* netstat failed */ }
    }

    private static bool IsOurOrthanc(Process process)
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