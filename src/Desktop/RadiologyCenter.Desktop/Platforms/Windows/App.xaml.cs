using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.Windows.AppLifecycle;

namespace RadiologyCenter.Desktop.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    private AppInstance? _instance;

	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		UpdateService.Init();
		// Set before WebView2 creation: setting its property later can still flash white.
		Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "FF4C58E0", EnvironmentVariableTarget.Process);
		this.InitializeComponent();
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
#if DEBUG
        // Keep independently built development/preview directories usable side by side.
        var suffix = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(AppContext.BaseDirectory.ToUpperInvariant())))[..16];
        var key = "EGcare.Desktop.Debug." + suffix;
#else
        const string key = "EGcare.Desktop";
#endif
        _instance = AppInstance.FindOrRegisterForKey(key);
        if (!_instance.IsCurrent)
        {
            try
            {
                await _instance.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs());
            }
            catch (Exception ex) { System.Diagnostics.Trace.TraceError("Could not restore the running EGcare instance: {0}", ex); }
            finally { Exit(); }
            return;
        }

        _instance.Activated += (_, _) => WindowsTrayService.Current?.Show();
        base.OnLaunched(args);
    }
}

