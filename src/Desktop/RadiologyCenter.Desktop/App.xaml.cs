namespace RadiologyCenter.Desktop;

using RadiologyCenter.Desktop.Services;
using WindowsColor = Windows.UI.Color;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public partial class App : Application
{
    private readonly PacsService _pacs;

    private readonly IServiceProvider _services;
    private Task? _shutdownTask;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        _pacs = new PacsService(services.GetRequiredService<IHttpClientFactory>(), services.GetRequiredService<IServiceScopeFactory>());
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window
        {
            Page = new MainPage(),
            Title = "EGcare",
            Width = 1212,
            Height = 735,
            MinimumWidth = 629,
            MinimumHeight = 624,
            TitleBar = new TitleBar
            {
                // Render the brand via custom leading content so we can control the
                // icon-to-name gap and make the name bold (the built-in Title/Icon use
                // the default template with fixed spacing and no bold).
                LeadingContent = new HorizontalStackLayout
                {
                    Spacing = 6,
                    Padding = new Thickness(12, 0, 0, 0),
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Image
                        {
                            Source = "healthcare_title_icon_16.png",
                            WidthRequest = 18,
                            HeightRequest = 18,
                            VerticalOptions = LayoutOptions.Center,
                        },
                        new Label
                        {
                            Text = "EGcare",
                            TextColor = Microsoft.Maui.Graphics.Colors.White,
                            FontFamily = "RobotoSlab",
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14,
                            VerticalOptions = LayoutOptions.Center,
                        },
                    },
                },
                ForegroundColor = Microsoft.Maui.Graphics.Colors.White,
                BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#4C58E0"),
            },
        };
        CenterWindowOnLaunch(window);
        window.Destroying += (_, _) =>
        {
            _services.GetRequiredService<WinUI.WindowsTrayService>().Dispose();
            _ = ShutdownAsync();
        };
        return window;
    }

    private Task ShutdownAsync() => _shutdownTask ??= ShutdownCoreAsync();

    private async Task ShutdownCoreAsync()
    {
        try { await _services.GetRequiredService<BusinessRuntimeService>().DisposeAsync(); }
        finally { _pacs.Dispose(); }
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
            try
            {
                _services.GetRequiredService<WinUI.WindowsTrayService>().Attach(native, ShutdownAsync);
            }
            catch (Exception ex)
            {
                // A tray failure must leave the normal visible window usable.
                _services.GetRequiredService<ILogger<App>>()
                    .LogError(ex, "Windows tray integration could not be initialized.");
            }

            // Center immediately so the window never flashes at the OS-default position.
            var area = Microsoft.UI.Windowing.DisplayArea
                .GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary).WorkArea;

            appWindow.Move(new Windows.Graphics.PointInt32(
                area.X + (area.Width - appWindow.Size.Width) / 2,
                area.Y + (area.Height - appWindow.Size.Height) / 2));

            // Defer until the native title bar binds. MAUI owns the AppWindow and may reset
            // the icon/colors if we apply them too early.
            native.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
            {
                ApplyMinWindowSize(appWindow);
                StyleCaptionButtons(appWindow);
                SetWindowIcon(appWindow);
            });
        };
#endif
    }

    private static void ApplyMinWindowSize(Microsoft.UI.Windowing.AppWindow appWindow)
    {
        if (appWindow.Presenter is not Microsoft.UI.Windowing.OverlappedPresenter presenter)
            return;

        // Keep the native presenter-based constraint AND set the MAUI Window-level minimums.
        presenter.PreferredMinimumWidth = 629;
        presenter.PreferredMinimumHeight = 624;
    }

    private static void StyleCaptionButtons(Microsoft.UI.Windowing.AppWindow appWindow)
    {
        if (appWindow.TitleBar is not Microsoft.UI.Windowing.AppWindowTitleBar bar)
            return;

        // Keep the system-drawn caption buttons but recolor them for the blue bar.
        // Hover/pressed behavior stays native; we only tint glyph + hover backgrounds.
        var white = WindowsColor.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
        bar.ButtonForegroundColor = white;
        bar.ButtonHoverForegroundColor = white;
        bar.ButtonHoverBackgroundColor = WindowsColor.FromArgb(0x26, 0xFF, 0xFF, 0xFF);
        bar.ButtonPressedForegroundColor = white;
        bar.ButtonPressedBackgroundColor = WindowsColor.FromArgb(0x40, 0xFF, 0xFF, 0xFF);
    }

    private static void SetWindowIcon(Microsoft.UI.Windowing.AppWindow appWindow)
    {
        var iconPath = Path.Combine(AppContext.BaseDirectory, "taskbar.ico");
        if (File.Exists(iconPath))
            appWindow.SetIcon(iconPath);
    }
}
