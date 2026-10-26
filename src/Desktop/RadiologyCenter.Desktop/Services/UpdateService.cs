using Velopack;
using Velopack.Sources;

namespace RadiologyCenter.Desktop;

public sealed class UpdateService
{

    private UpdateManager? _mgr;
    private string? _sourceDirectory;

    public static void Init()
    {
        VelopackApp.Build()
            .SetAutoApplyOnStartup(false)
            .OnAfterInstallFastCallback(_ => RunStartupHook(() => WinUI.WindowsStartupRegistration.SetEnabled(true)))
            .OnAfterUpdateFastCallback(_ => RunStartupHook(WinUI.WindowsStartupRegistration.EnsureRegistered))
            .OnBeforeUninstallFastCallback(_ => RunStartupHook(WinUI.WindowsStartupRegistration.Remove))
            .Run();
    }

    private static void RunStartupHook(Action action)
    {
        try { action(); }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceWarning("EGcare Windows startup registration: {0}", ex);
        }
    }

    public UpdateManager GetManager()
    {
        var directory=Services.DesktopConfiguration.Profile?.UpdatesDirectory
            ?? throw new InvalidOperationException("No clinic update folder is configured.");
        if (_mgr is null || !string.Equals(directory,_sourceDirectory,StringComparison.OrdinalIgnoreCase))
        {
            _mgr=new UpdateManager(new SimpleFileSource(new DirectoryInfo(directory)));
            _sourceDirectory=directory;
        }
        return _mgr;
    }

    public async Task<UpdateInfo?> CheckForUpdatesAsync(bool throwOnError = false)
    {
        try
        {
            var directory = Services.DesktopConfiguration.Profile?.UpdatesDirectory;
            if (string.IsNullOrWhiteSpace(directory))
                throw new InvalidOperationException("Choose a clinic update folder in the connection settings first.");
            var manager = GetManager();
            if (!manager.IsInstalled)
                throw new InvalidOperationException("Updates are available after installing EGcare with the clinic installer. Development builds cannot install updates.");
            if (!Directory.Exists(directory))
                throw new InvalidOperationException("The clinic update folder is unavailable. Check the local network and folder access.");
            return await manager.CheckForUpdatesAsync();
        }
        catch when (!throwOnError)
        {
            return null;
        }
    }

    public async Task<bool> DownloadUpdateAsync(UpdateInfo update, Action<int>? progress = null)
    {
        try
        {
            await GetManager().DownloadUpdatesAsync(update, progress);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void ApplyAndRestart(UpdateInfo update)
    {
        GetManager().ApplyUpdatesAndRestart(update);
    }
}
