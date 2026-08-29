using System.Globalization;
using System.Text.Json;

namespace RadiologyCenter.Desktop.Localization;

/// <summary>
/// Lightweight JSON-based localizer for the desktop client.
/// Supports English and Arabic, persists the user's choice, and flips
/// the UI direction (LTR/RTL) when the culture changes.
/// </summary>
public sealed partial class AppLocalizer
{
    public const string DefaultCulture = "en";
    public const string ArabicCulture = "ar";
    public const string CulturePreferenceKey = "app.culture";

    private static readonly string[] RtlCultures = { "ar", "he", "fa", "ur" };
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly Dictionary<string, Dictionary<string, string>> _resources = new(StringComparer.Ordinal);

    public AppLocalizer()
    {
        LoadResources();
        CurrentCulture = ResolveInitialCulture();
        ApplyCulture();
    }

    public event Action? LanguageChanged;

    public string CurrentCulture { get; private set; }

    public bool IsRTL => RtlCultures.Contains(CurrentCulture);

    public string this[string key] => Get(key);

    public string Get(string key, string? fallback = null)
    {
        if (TryGet(key, out var value))
            return value;
        return fallback ?? key;
    }

    public string FormatValue(string template, params object?[] args)
    {
        if (args.Length == 0)
            return template;
        try
        {
            return string.Format(template, args);
        }
        catch (FormatException)
        {
            return template;
        }
    }

    public void SetCulture(string culture)
    {
        culture = NormalizeCulture(culture);
        if (culture == CurrentCulture)
            return;

        CurrentCulture = culture;
        Preferences.Default.Set(CulturePreferenceKey, culture);
        ApplyCulture();
        LanguageChanged?.Invoke();
    }

    private void ApplyCulture()
    {
        var cultureInfo = new CultureInfo(CurrentCulture);
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }

    private static string ResolveInitialCulture()
    {
        var saved = Preferences.Default.Get(CulturePreferenceKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(saved))
            return NormalizeCulture(saved);

        var uiCulture = CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;
        return RtlCultures.Contains(uiCulture.TwoLetterISOLanguageName) ? ArabicCulture : DefaultCulture;
    }

    private static string NormalizeCulture(string culture)
    {
        var code = (culture ?? string.Empty).Trim().ToLowerInvariant();
        return code.StartsWith("ar", StringComparison.Ordinal) ? ArabicCulture : DefaultCulture;
    }

    private bool TryGet(string key, out string value)
    {
        if (_resources.TryGetValue(CurrentCulture, out var dict) && dict.TryGetValue(key, out var localized))
        {
            value = localized;
            return true;
        }

        if (!string.Equals(CurrentCulture, DefaultCulture, StringComparison.Ordinal)
            && _resources.TryGetValue(DefaultCulture, out var en) && en.TryGetValue(key, out var english))
        {
            value = english;
            return true;
        }

        value = key;
        return false;
    }

    private void LoadResources()
    {
        foreach (var culture in new[] { DefaultCulture, ArabicCulture })
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Localization", $"{culture}.json");
            if (!File.Exists(path))
                continue;

            try
            {
                var json = File.ReadAllText(path);
                var nested = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, JsonOptions);
                if (nested is null || nested.Count == 0)
                    continue;

                var flat = new Dictionary<string, string>(StringComparer.Ordinal);
                Flatten(nested, null, flat);
                _resources[culture] = flat;
            }
            catch (JsonException)
            {
                // A malformed resource file must not prevent the app from starting.
            }
        }
    }

    private static void Flatten(Dictionary<string, JsonElement> nodes, string? prefix, Dictionary<string, string> flat)
    {
        foreach (var kv in nodes)
        {
            var key = prefix is null ? kv.Key : $"{prefix}.{kv.Key}";
            if (kv.Value.ValueKind == JsonValueKind.Object)
            {
                var obj = kv.Value.Deserialize<Dictionary<string, JsonElement>>(JsonOptions);
                if (obj is not null)
                    Flatten(obj, key, flat);
            }
            else
            {
                flat[key] = kv.Value.GetString() ?? string.Empty;
            }
        }
    }
}
