using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using NativeWindow = Microsoft.UI.Xaml.Window;

namespace RadiologyCenter.Desktop.WinUI;

internal sealed class WindowsTrayService(AppLocalizer text, ILogger<WindowsTrayService> logger) : IDisposable
{
    private const uint TrayMessage = 0x8000 + 47;
    private const uint IconId = 1;
    private const nuint SubclassId = 0x454743;
    private const uint OpenCommand = 1, ExitCommand = 2, StartupCommand = 3;
    private const uint WmNull = 0, WmContextMenu = 0x007B, WmQueryEndSession = 0x0011, WmEndSession = 0x0016;
    private const uint NinSelect = 0x0400, NinKeySelect = 0x0401;
    private const uint NimAdd = 0, NimDelete = 2, NimSetFocus = 3, NimSetVersion = 4;
    private const uint NifMessage = 1, NifIcon = 2, NifTip = 4, NifShowTip = 0x80;

    private readonly ILogger<WindowsTrayService> _logger = logger;
    private readonly SubclassProcedure _subclass = WindowProcedure;
    private static readonly Dictionary<nint, WindowsTrayService> Instances = new();
    private NativeWindow? _window;
    private AppWindow? _appWindow;
    private Func<Task>? _shutdown;
    private nint _hwnd, _icon;
    private uint _taskbarCreated;
    private bool _iconAdded, _ownsIcon, _exitRequested, _systemEnding, _disposed;

    public static WindowsTrayService? Current { get; private set; }

    public void Attach(NativeWindow window, Func<Task> shutdown)
    {
        if (_window is not null) return;
        _window = window;
        _appWindow = window.AppWindow;
        _shutdown = shutdown;
        _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        _taskbarCreated = RegisterWindowMessage("TaskbarCreated");
        Instances.Add(_hwnd, this);
        if (!SetWindowSubclass(_hwnd, _subclass, SubclassId, 0))
        {
            Instances.Remove(_hwnd);
            _window = null;
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not attach the EGcare tray icon.");
        }

        Current = this;
        LoadTrayIcon();
        AddIcon();
        _appWindow.Closing += OnClosing;
        window.Closed += OnClosed;
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        try { WindowsStartupRegistration.EnsureRegistered(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Windows startup registration could not be updated."); }
    }

    public void Show()
    {
        if (_disposed || _exitRequested || _window is null) return;
        _window.DispatcherQueue.TryEnqueue(() =>
        {
            if (_disposed || _exitRequested) return;
            _appWindow!.Show();
            if (_appWindow.Presenter is OverlappedPresenter { State: OverlappedPresenterState.Minimized } presenter)
                presenter.Restore();
            _window.Activate();
            SetForegroundWindow(_hwnd);
        });
    }

    private void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_exitRequested || _systemEnding) return;
        // If Explorer/tray is unavailable, leave normal closing available; never strand a hidden app.
        if (!_iconAdded && !AddIcon()) return;
        args.Cancel = true;
        sender.Hide();
    }

    private void OnClosed(object sender, Microsoft.UI.Xaml.WindowEventArgs args) => Dispose();
    private void OnProcessExit(object? sender, EventArgs args) => RemoveIcon();

    private async Task ExitAsync()
    {
        if (_exitRequested) return;
        _exitRequested = true;
        try
        {
            if (_shutdown is not null) await _shutdown();
        }
        catch (Exception ex) { _logger.LogError(ex, "EGcare shutdown cleanup did not complete."); }
        finally
        {
            RemoveIcon();
            _window?.Close();
        }
    }

    private void LoadTrayIcon()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "taskbar.ico");
        var dpi = GetDpiForWindow(_hwnd);
        _icon = LoadImage(0, path, 1, GetSystemMetricsForDpi(49, dpi), GetSystemMetricsForDpi(50, dpi), 0x0010);
        _ownsIcon = _icon != 0;
        if (_icon == 0) _icon = LoadIcon(0, (nint)32512);
    }

    private NotifyIconData IconData() => new()
    {
        Size = (uint)Marshal.SizeOf<NotifyIconData>(), Window = _hwnd, Id = IconId,
        Flags = NifMessage | NifIcon | NifTip | NifShowTip, CallbackMessage = TrayMessage,
        Icon = _icon, Tip = "EGcare", Info = string.Empty, InfoTitle = string.Empty
    };

    private bool AddIcon()
    {
        if (_disposed || _icon == 0) return false;
        var data = IconData();
        _iconAdded = ShellNotifyIcon(NimAdd, ref data);
        if (_iconAdded)
        {
            data.Version = 4;
            ShellNotifyIcon(NimSetVersion, ref data);
        }
        else _logger.LogWarning("The Windows notification area is unavailable; close-to-tray is disabled until it returns.");
        return _iconAdded;
    }

    private void RemoveIcon()
    {
        if (!_iconAdded) return;
        var data = IconData();
        ShellNotifyIcon(NimDelete, ref data);
        _iconAdded = false;
    }

    private void ShowMenu(nint coordinates)
    {
        var menu = CreatePopupMenu();
        if (menu == 0) return;
        uint command;
        try
        {
            AppendMenu(menu, 0, OpenCommand, text["desktop.open"]);
            SetMenuDefaultItem(menu, OpenCommand, false);
            if (WindowsStartupRegistration.IsInstalled)
            {
                AppendMenu(menu, 0x0800, 0, null);
                AppendMenu(menu, WindowsStartupRegistration.IsEnabled ? 0x0008u : 0, StartupCommand, text["desktop.startWithWindows"]);
            }
            AppendMenu(menu, 0x0800, 0, null);
            AppendMenu(menu, 0, ExitCommand, text["desktop.exit"]);
            var point = new NativePoint { X = (short)((long)coordinates & 0xffff), Y = (short)(((long)coordinates >> 16) & 0xffff) };
            if (point.X == -1 && point.Y == -1) GetCursorPos(out point);
            SetForegroundWindow(_hwnd);
            command = TrackPopupMenuEx(menu, 0x0100 | 0x0002 | (text.IsRTL ? 0x8000u : 0), point.X, point.Y, _hwnd, 0);
            PostMessage(_hwnd, WmNull, 0, 0);
            var data = IconData();
            ShellNotifyIcon(NimSetFocus, ref data);
        }
        finally { DestroyMenu(menu); }

        if (command == OpenCommand) Show();
        else if (command == ExitCommand) _window!.DispatcherQueue.TryEnqueue(() => _ = ExitAsync());
        else if (command == StartupCommand)
        {
            try { WindowsStartupRegistration.SetEnabled(!WindowsStartupRegistration.IsEnabled); }
            catch (Exception ex) { _logger.LogWarning(ex, "The Windows startup preference could not be changed."); }
        }
    }

    private static nint WindowProcedure(nint hwnd, uint message, nuint wParam, nint lParam, nuint id, nuint data)
    {
        if (Instances.TryGetValue(hwnd, out var service))
        {
            try
            {
                if (message == service._taskbarCreated && message != 0)
                {
                    service._iconAdded = false;
                    if (!service.AddIcon()) service.Show();
                }
                else if (message == WmQueryEndSession) service._systemEnding = true;
                else if (message == WmEndSession) service._systemEnding = wParam != 0;
                else if (message == TrayMessage)
                {
                    var notification = (uint)((long)lParam & 0xffff);
                    if (notification is NinSelect or NinKeySelect) service.Show();
                    else if (notification == WmContextMenu) service.ShowMenu((nint)wParam);
                    return 0;
                }
            }
            catch (Exception ex) { service._logger.LogError(ex, "A Windows tray action failed."); }
        }
        return DefSubclassProc(hwnd, message, wParam, lParam);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        RemoveIcon();
        if (_appWindow is not null) _appWindow.Closing -= OnClosing;
        if (_window is not null) _window.Closed -= OnClosed;
        AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
        RemoveWindowSubclass(_hwnd, _subclass, SubclassId);
        Instances.Remove(_hwnd);
        if (_ownsIcon) DestroyIcon(_icon);
        if (ReferenceEquals(Current, this)) Current = null;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NotifyIconData
    {
        public uint Size;
        public nint Window;
        public uint Id, Flags, CallbackMessage;
        public nint Icon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string Tip;
        public uint State, StateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Info;
        public uint Version;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string InfoTitle;
        public uint InfoFlags;
        public Guid Guid;
        public nint BalloonIcon;
    }

    [StructLayout(LayoutKind.Sequential)] private struct NativePoint { public int X, Y; }
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint SubclassProcedure(nint hwnd, uint message, nuint wParam, nint lParam, nuint id, nuint data);

    [DllImport("comctl32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowSubclass(nint window, SubclassProcedure callback, nuint id, nuint data);
    [DllImport("comctl32.dll")] [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveWindowSubclass(nint window, SubclassProcedure callback, nuint id);
    [DllImport("comctl32.dll")] private static extern nint DefSubclassProc(nint window, uint message, nuint wParam, nint lParam);
    [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW", CharSet = CharSet.Unicode)] [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShellNotifyIcon(uint operation, ref NotifyIconData data);
    [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW", CharSet = CharSet.Unicode)] private static extern uint RegisterWindowMessage(string message);
    [DllImport("user32.dll", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode)] private static extern nint LoadImage(nint instance, string path, uint type, int width, int height, uint flags);
    [DllImport("user32.dll", EntryPoint = "LoadIconW")] private static extern nint LoadIcon(nint instance, nint resource);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool DestroyIcon(nint icon);
    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(nint window);
    [DllImport("user32.dll")] private static extern int GetSystemMetricsForDpi(int index, uint dpi);
    [DllImport("user32.dll")] private static extern nint CreatePopupMenu();
    [DllImport("user32.dll", EntryPoint = "AppendMenuW", CharSet = CharSet.Unicode)] [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AppendMenu(nint menu, uint flags, nuint id, string? label);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool SetMenuDefaultItem(nint menu, uint item, [MarshalAs(UnmanagedType.Bool)] bool byPosition);
    [DllImport("user32.dll")] private static extern uint TrackPopupMenuEx(nint menu, uint flags, int x, int y, nint owner, nint parameters);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool DestroyMenu(nint menu);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool GetCursorPos(out NativePoint point);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool SetForegroundWindow(nint window);
    [DllImport("user32.dll", EntryPoint = "PostMessageW")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool PostMessage(nint window, uint message, nuint wParam, nint lParam);
}
