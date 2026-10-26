using Velopack;

namespace RadiologyCenter.Desktop;

public static class UpdateService
{

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

}
