using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RadiologyCenter.Desktop;

public sealed class LocalhostService : IDisposable
{
    private const int Port = 5224;
    private const int StartupTimeoutMs = 10000;
    private const int HealthCheckIntervalMs = 500;

    public static LocalhostService? Instance { get; private set; }

#if WINDOWS
    private Process? _process;
#endif

    private Task? _startupTask;
    private string? _startupError;

    public bool IsReady => _startupTask is { IsCompletedSuccessfully: true };
    public string? StartupError => _startupError;

    public LocalhostService()
    {
        Instance = this;
#if WINDOWS
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Dispose();
#endif
    }

    public Task StartAsync()
    {
        if (_startupTask is not null && !_startupTask.IsCanceled)
            return _startupTask;

        _startupError = null;
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
#if WINDOWS
        try
        {
            EnsurePortIsFree();
        var (exePath, workDir, isDevelopment) = ResolveLocalhostPaths();

        var stderr = new StringWriter();

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = isDevelopment ? "dotnet" : exePath,
                Arguments = isDevelopment ? $"\"{exePath}\" --urls http://localhost:{Port}" : $"--urls http://localhost:{Port}",
                WorkingDirectory = workDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                EnvironmentVariables =
                {
                    ["ASPNETCORE_ENVIRONMENT"] = isDevelopment ? "Development" : "Production",
                    ["ASPNETCORE_URLS"] = $"http://localhost:{Port}",
                },
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
                _process.Kill(entireProcessTree: true);
                var error = stderr.ToString();
                throw new InvalidOperationException(
                    $"Localhost failed to start within {StartupTimeoutMs}ms.\nError output:\n{error}");
            }
        }
        catch (Exception ex)
        {
            _startupError = ex.Message;
            throw;
        }
#endif
    }

    public void Stop()
    {
#if WINDOWS
        KillProcess();
        EnsurePortIsFree();
#endif
    }

    public void Dispose() => Stop();

#if WINDOWS
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
                var response = await httpClient.GetAsync($"http://localhost:{Port}/health");
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

            foreach (Match match in Regex.Matches(output, $":{Port}\\s+\\S+\\s+LISTENING\\s+(\\d+)"))
            {
                var pid = int.Parse(match.Groups[1].Value);
                if (pid <= 0) continue;

                try
                {
                    using var victim = Process.GetProcessById(pid);
                    if (!IsOurBackend(victim))
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
        catch { /* netstat failed */ }
    }

    private static bool IsOurBackend(Process process)
    {
        try
        {
            if (process.ProcessName.Contains("RadiologyCenter.Localhost", StringComparison.OrdinalIgnoreCase))
                return true;

            var path = process.MainModule?.FileName;
            return path is not null &&
                   path.Contains("RadiologyCenter.Localhost", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static (string exePath, string workDir, bool isDevelopment) ResolveLocalhostPaths()
    {
        var productionExe = Path.Combine(AppContext.BaseDirectory, "localhost", "RadiologyCenter.Localhost.exe");
        if (File.Exists(productionExe))
            return (productionExe, Path.Combine(AppContext.BaseDirectory, "localhost"), false);

        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "Backend", "Localhost")))
            dir = dir.Parent;

        if (dir?.FullName is not { } root)
            throw new InvalidOperationException("Solution root not found.");

        var localhostDir = Path.Combine(root, "Backend", "Localhost", "RadiologyCenter.Localhost");
        var config =
#if DEBUG
        "Debug";
#else
        "Release";
#endif
        var dll = Path.Combine(localhostDir, "bin", config, "net10.0", "RadiologyCenter.Localhost.dll");

        if (!File.Exists(dll))
            throw new FileNotFoundException(
                "Localhost DLL not found. Build the Localhost project first.", dll);

        return (dll, localhostDir, true);
    }
#endif
}
