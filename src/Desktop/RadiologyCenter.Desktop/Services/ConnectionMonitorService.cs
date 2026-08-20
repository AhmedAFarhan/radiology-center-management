using Microsoft.Extensions.Logging;
using Windows.Networking.Connectivity;

namespace RadiologyCenter.Desktop.Services;

public sealed class ConnectionMonitorService : IDisposable
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(3);

    private readonly ILogger<ConnectionMonitorService> _logger;
    private readonly object _gate = new();
    private bool _started;
    private bool _disposed;
    private bool _lastOnline;

    public ConnectionMonitorService(ILogger<ConnectionMonitorService> logger)
    {
        _logger = logger;
    }

    public event Action? StateChanged;

    public bool IsOnline => GetOnline();

    public void Start()
    {
        lock (_gate)
        {
            if (_started)
                return;
            _started = true;
            _lastOnline = GetOnline();
        }

        _logger.LogInformation("Connection monitor started. Initial online state: {Online}", _lastOnline);
        NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
        _ = PollLoopAsync();
    }

    public bool CheckNow()
    {
        RaiseIfChanged();
        return IsOnline;
    }

    private static bool GetOnline()
    {
        try
        {
            return NetworkInformation.GetInternetConnectionProfile() is not null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void OnNetworkStatusChanged(object sender)
        => RaiseIfChanged();

    private async Task PollLoopAsync()
    {
        while (!_disposed)
        {
            await Task.Delay(PollInterval);
            if (_disposed)
                return;
            RaiseIfChanged();
        }
    }

    private void RaiseIfChanged()
    {
        var online = GetOnline();
        bool changed;
        lock (_gate)
        {
            changed = online != _lastOnline;
            _lastOnline = online;
        }

        if (changed)
        {
            _logger.LogInformation("Network connectivity changed. Online: {Online}", online);
            StateChanged?.Invoke();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
    }
}