using RadiologyCenter.Desktop.Security;

namespace RadiologyCenter.Desktop.Services;
public sealed class BackendStatusService(BusinessRuntimeService business, TokenStorage tokens)
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
            await RestoreSessionAsync();
            IsReady=true;
        } catch (Exception ex) { IsFailed=true; Error=ex.Message; }
        finally { IsStarting=false; StateChanged?.Invoke(); _start.Release(); }
    }
    public Task RetryAsync() => StartAsync();

    private async Task RestoreSessionAsync()
    {
        if (tokens.GetTokens() is not { } saved) return;
        try
        {
            var restored = await business.RestoreSessionAsync(saved, tokens.GetDatabasePassword());
            if (restored.MustChangePassword)
                throw new ApiException(401, "Change your password before restoring this session.");
            tokens.Save(AuthTokens.From(restored, saved.Username));
        }
        catch (ApiException ex) when (ex.StatusCode is 401 or 423)
        {
            // Expired, revoked, locked or changed credentials require an explicit sign-in.
            tokens.Clear();
            await business.SignOutAsync();
        }
        // Connection failures reach the startup retry screen and preserve the saved session.
    }
}
