namespace RadiologyCenter.Desktop.Services;
public sealed class BackendStatusService(BusinessRuntimeService business)
{
    public event Action? StateChanged;
    public bool IsReady { get; private set; }
    public bool IsFailed { get; private set; }
    public string? Error { get; private set; }
    public bool IsStarting { get; private set; }
    public bool NeedsSetup => !DesktopConfiguration.IsDevelopment && !File.Exists(DesktopConfiguration.ProfilePath);
    private readonly SemaphoreSlim _start = new(1,1);
    public Task EnsureStartedAsync() => IsReady ? Task.CompletedTask : StartAsync();
    public async Task StartAsync()
    {
        await _start.WaitAsync();
        try {
            IsStarting=true; IsReady=false; IsFailed=false; Error=null; StateChanged?.Invoke();
            if (NeedsSetup) return;
            if (DesktopConfiguration.IsDevelopment) await business.EnsureDevelopmentAsync();
            else (DesktopConfiguration.Profile ?? throw new InvalidOperationException("The clinic connection file is invalid. Open clinic setup and import or correct it.")).Validate();
            // SQL credentials are requested by the login dialog, before opening the shared database.
            IsReady=true;
        } catch (Exception ex) { IsFailed=true; Error=ex.Message; }
        finally { IsStarting=false; StateChanged?.Invoke(); _start.Release(); }
    }
    public Task RetryAsync() => StartAsync();
}
