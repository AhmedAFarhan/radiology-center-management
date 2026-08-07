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
        var window = new Window
        {
            Page = new MainPage(),
            Title = "EGcare",
            TitleBar = new TitleBar
            {
                Title = "EGcare",
                Icon = "healthcare_title_icon_16.png",
                ForegroundColor = Colors.White,
                BackgroundColor = Color.FromArgb("#4C58E0"),
            },
        };
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

            // Center immediately so the window never flashes at the OS-default position.
            var area = Microsoft.UI.Windowing.DisplayArea
                .GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary).WorkArea;

            appWindow.Move(new Windows.Graphics.PointInt32(
                area.X + (area.Width - appWindow.Size.Width) / 2,
                area.Y + (area.Height - appWindow.Size.Height) / 2));

            // Defer only the icon until the native title bar binds. MAUI owns the
            // AppWindow and may reset the icon if we set it too early.
            native.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                SetWindowIcon(appWindow));
        };
#endif
    }

    private static void SetWindowIcon(Microsoft.UI.Windowing.AppWindow appWindow)
    {
        var iconPath = Path.Combine(AppContext.BaseDirectory, "taskbar.ico");
        if (File.Exists(iconPath))
            appWindow.SetIcon(iconPath);
    }
}