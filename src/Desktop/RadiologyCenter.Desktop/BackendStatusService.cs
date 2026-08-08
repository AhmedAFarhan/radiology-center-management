namespace RadiologyCenter.Desktop;

public sealed class BackendStatusService
{
    public event Action? StateChanged;

    public bool IsReady { get; private set; }
    public bool IsFailed { get; private set; }
    public string? Error { get; private set; }
    public bool IsStarting { get; private set; }

    private bool _started;

    public async Task EnsureStartedAsync()
    {
        if (_started)
            return;

        _started = true;
        await StartAsync();
    }

    public async Task StartAsync()
    {
        IsStarting = true;
        IsReady = false;
        IsFailed = false;
        Error = null;
        StateChanged?.Invoke();

        await Task.Delay(TimeSpan.FromSeconds(1));

        try
        {
            await (LocalhostService.Instance?.StartAsync() ?? Task.CompletedTask);
            IsReady = LocalhostService.Instance?.IsReady ?? true;
        }
        catch (Exception ex)
        {
            IsFailed = true;
            Error = ex.Message;
        }
        finally
        {
            IsStarting = false;
        }

        StateChanged?.Invoke();
    }

    public async Task RetryAsync()
    {
        try
        {
            await (LocalhostService.Instance?.RetryAsync() ?? Task.CompletedTask);
            IsReady = LocalhostService.Instance?.IsReady ?? true;
            IsFailed = false;
            Error = null;
        }
        catch (Exception ex)
        {
            IsFailed = true;
            Error = ex.Message;
        }

        StateChanged?.Invoke();
    }
}
