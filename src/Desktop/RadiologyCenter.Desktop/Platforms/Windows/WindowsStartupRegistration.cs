using Microsoft.Win32;
using Velopack.Locators;

namespace RadiologyCenter.Desktop.WinUI;

internal static class WindowsStartupRegistration
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string SettingsKey = @"Software\EGcare\Desktop";
    private const string ValueName = "EGcare";
    private const string PreferenceName = "StartWithWindows";

    public static bool IsInstalled => VelopackLocator.IsCurrentSet
        && VelopackLocator.Current.CurrentlyInstalledVersion is not null
        && !VelopackLocator.Current.IsPortable;

    public static bool IsEnabled
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey);
            return key?.GetValue(ValueName) is string;
        }
    }

    public static void EnsureRegistered()
    {
        // Never register Visual Studio output, temporary previews or portable copies.
        if (!IsInstalled) return;
        using var settings = Registry.CurrentUser.CreateSubKey(SettingsKey);
        if (settings.GetValue(PreferenceName) is null)
            SetEnabled(true);
        else if (IsEnabled)
            WriteStartupCommand();
    }

    public static void SetEnabled(bool enabled)
    {
        if (!IsInstalled) return;
        if (enabled)
            WriteStartupCommand();
        else
            RemoveStartupCommand();

        using var settings = Registry.CurrentUser.CreateSubKey(SettingsKey);
        settings.SetValue(PreferenceName, enabled ? 1 : 0, RegistryValueKind.DWord);
    }

    public static void Remove()
    {
        if (!IsInstalled) return;
        RemoveStartupCommand();
        using var settings = Registry.CurrentUser.OpenSubKey(SettingsKey, writable: true);
        settings?.DeleteValue(PreferenceName, throwOnMissingValue: false);
    }

    private static void WriteStartupCommand()
    {
        var locator = VelopackLocator.Current;
        if (string.IsNullOrWhiteSpace(locator.UpdateExePath) || !File.Exists(locator.UpdateExePath))
            throw new FileNotFoundException("The installed EGcare launcher could not be found.");
        if (string.IsNullOrWhiteSpace(locator.ThisExeRelativePath))
            throw new InvalidOperationException("The installed EGcare executable could not be identified.");

        // Update.exe stays at the installation root while versioned app files change.
        var command = $"\"{locator.UpdateExePath}\" start \"{locator.ThisExeRelativePath}\"";
        using var key = Registry.CurrentUser.CreateSubKey(RunKey);
        key.SetValue(ValueName, command, RegistryValueKind.String);
    }

    private static void RemoveStartupCommand()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        key?.DeleteValue(ValueName, throwOnMissingValue: false);
    }
}
