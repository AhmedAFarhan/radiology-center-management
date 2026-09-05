using System.Diagnostics;

namespace RadiologyCenter.Localhost.Services;

/// <summary>
/// Monitors the parent process (Desktop app) and shuts down this backend
/// when the parent exits — prevents orphaned processes during development.
/// </summary>
internal sealed class ParentProcessWatcher : IHostedService, IDisposable
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<ParentProcessWatcher> _logger;
    private readonly int _parentPid;
    private Timer? _timer;

    public ParentProcessWatcher(IHostApplicationLifetime lifetime, ILogger<ParentProcessWatcher> logger, IConfiguration config)
    {
        _lifetime = lifetime;
        _logger = logger;
        _parentPid = config.GetValue<int>("ParentPid");
    }

    public Task StartAsync(CancellationToken ct)
    {
        if (_parentPid <= 0)
            return Task.CompletedTask;

        _timer = new Timer(CheckParent, null, 2000, 2000);
        _logger.LogDebug("Watching parent process {ParentPid}", _parentPid);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    private void CheckParent(object? state)
    {
        try
        {
            using var parent = Process.GetProcessById(_parentPid);
            if (parent.HasExited)
            {
                _logger.LogWarning("Parent process {ParentPid} exited — shutting down", _parentPid);
                _lifetime.StopApplication();
            }
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("Parent process {ParentPid} not found — shutting down", _parentPid);
            _lifetime.StopApplication();
        }
    }

    public void Dispose() => _timer?.Dispose();
}
