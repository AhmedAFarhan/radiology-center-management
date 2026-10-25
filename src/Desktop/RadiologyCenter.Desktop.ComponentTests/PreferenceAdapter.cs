namespace RadiologyCenter.Desktop.Localization;

// The component and real JSON localizer run without a MAUI platform in these tests.
// Preference persistence is the only native dependency replaced here.
internal sealed class Preferences
{
    internal static Preferences Default { get; } = new();
    public string Get(string key, string fallback) => fallback;
    public void Set(string key, string value) { }
}
