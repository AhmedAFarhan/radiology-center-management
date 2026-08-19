using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RadiologyCenter.Desktop;

/// <summary>
/// Shared process lifecycle plumbing for services that spawn and supervise a
/// local backend process (the ASP.NET localhost server and the Orthanc PACS
/// server). Derived services supply the process command line, the ports to
/// free, the health-check URL and the process-name matcher; the base handles
/// start, retry, health polling, process kill and port cleanup.
/// </summary>
public abstract class LocalProcessServiceBase : IDisposable
{
    private Process? _process;
    private Task? _startupTask;
    private string? _startupError;

    public bool IsReady => _startupTask is { IsCompletedSuccessfully: true };
    public string? StartupError => _startupError;

    protected abstract string StartFailureMessage { get; }
    protected abstract string HealthCheckUrl { get; }
    protected abstract IReadOnlyList<int> Ports { get; }
    protected abstract int StartupTimeoutMs { get; }
    protected abstract int HealthCheckIntervalMs { get; }

    protected LocalProcessServiceBase()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Dispose();
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
        try
        {
            EnsurePortIsFree();

            var stderr = new StringWriter();
            _process = CreateProcess();
            _process.EnableRaisingEvents = true;
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
                    $"{StartFailureMessage}\nError output:\n{error}");
            }
        }
        catch (Exception ex)
        {
            _startupError = ex.Message;
            throw;
        }
    }

    public void Stop()
    {
        KillProcess();
        EnsurePortIsFree();
    }

    public void Dispose() => Stop();

    /// <summary>Builds a configured but not-yet-started process.</summary>
    protected abstract Process CreateProcess();

    /// <summary>Returns true when the process belongs to this service (and may be killed).</summary>
    protected abstract bool IsOwnProcess(Process process);

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
                var response = await httpClient.GetAsync(HealthCheckUrl);
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

    private void EnsurePortIsFree()
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

            foreach (var port in Ports)
            {
                foreach (Match match in Regex.Matches(output, $":{port}\\s+\\S+\\s+LISTENING\\s+(\\d+)"))
                {
                    var pid = int.Parse(match.Groups[1].Value);
                    if (pid <= 0) continue;

                    try
                    {
                        using var victim = Process.GetProcessById(pid);
                        if (!IsOwnProcess(victim))
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
}