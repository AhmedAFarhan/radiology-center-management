using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RadiologyCenter.Desktop;

public sealed class LocalhostService : IDisposable
{
    private const int Port = 5224;

#if WINDOWS
    private Process? _process;
#endif

    public LocalhostService()
    {
#if WINDOWS
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Dispose();
#endif
    }

    public Task StartAsync()
    {
#if WINDOWS
        EnsurePortIsFree();
        var (exePath, workDir, isDevelopment) = ResolveLocalhostPaths();

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = isDevelopment ? "dotnet" : exePath,
                Arguments = isDevelopment ? $"\"{exePath}\" --urls http://localhost:{Port}" : $"--urls http://localhost:{Port}",
                WorkingDirectory = workDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                EnvironmentVariables =
                {
                    ["ASPNETCORE_ENVIRONMENT"] = isDevelopment ? "Development" : "Production",
                    ["ASPNETCORE_URLS"] = $"http://localhost:{Port}",
                },
            },
            EnableRaisingEvents = true,
        };

        _process.Start();
#endif
        return Task.CompletedTask;
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
                try { Process.GetProcessById(int.Parse(match.Groups[1].Value)).Kill(entireProcessTree: true); }
                catch { /* already gone */ }
            }
        }
        catch { /* netstat failed */ }
    }

    private static (string exePath, string workDir, bool isDevelopment) ResolveLocalhostPaths()
    {
        // Production: Localhost published alongside the Desktop app
        var productionExe = Path.Combine(AppContext.BaseDirectory, "localhost", "RadiologyCenter.Localhost.exe");
        if (File.Exists(productionExe))
            return (productionExe, Path.Combine(AppContext.BaseDirectory, "localhost"), false);

        // Development: walk up to find the solution root
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
