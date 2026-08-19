using System.Diagnostics;

namespace RadiologyCenter.Desktop;

public sealed class LocalhostService : LocalProcessServiceBase
{
    private const int Port = 5224;

    public static LocalhostService? Instance { get; private set; }

    protected override string StartFailureMessage
        => $"Localhost failed to start within {StartupTimeoutMs}ms.";

    protected override string HealthCheckUrl
        => $"http://localhost:{Port}/health";

    protected override IReadOnlyList<int> Ports => new[] { Port };

    protected override int StartupTimeoutMs => 10000;

    protected override int HealthCheckIntervalMs => 500;

    public LocalhostService()
    {
        Instance = this;
    }

    protected override Process CreateProcess()
    {
        var (exePath, workDir, isDevelopment) = ResolveLocalhostPaths();

        return new Process
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
        };
    }

    protected override bool IsOwnProcess(Process process)
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
}