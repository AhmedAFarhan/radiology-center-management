namespace RadiologyCenter.Desktop;

public partial class App : Application
{
    private readonly LocalhostService _localhost;

    public App()
    {
        InitializeComponent();
        _localhost = new LocalhostService();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage()) { Title = "EGcare" };
        CenterWindowOnLaunch(window);
        window.Destroying += (_, _) => _localhost.Stop();
        return window;
    }

    private void CenterWindowOnLaunch(Window window)
    {
#if WINDOWS
        window.HandlerChanged += (_, _) =>
        {
            if (window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window native)
                return;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(native);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            var iconPath = Path.Combine(AppContext.BaseDirectory, "taskbar.ico");
            if (File.Exists(iconPath))
                appWindow.SetIcon(iconPath);

            var area = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary).WorkArea;

            appWindow.Move(new Windows.Graphics.PointInt32(
                area.X + (area.Width - appWindow.Size.Width) / 2,
                area.Y + (area.Height - appWindow.Size.Height) / 2));
        };
#endif
    }

    protected override void OnStart()
    {
        _ = _localhost.StartAsync();
    }
}
