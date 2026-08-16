using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Localization;

namespace RadiologyCenter.Localhost.Localization;

public sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _resources;
    private readonly ConcurrentDictionary<string, JsonStringLocalizer> _cache = new();

    public JsonStringLocalizerFactory(IWebHostEnvironment environment)
    {
        _resources = LoadResources(environment);
    }

    public IStringLocalizer Create(Type resourceSource) => Create(resourceSource.Name);

    public IStringLocalizer Create(string baseName)
        => _cache.GetOrAdd(baseName, _ => new JsonStringLocalizer(_resources));

    public IStringLocalizer Create(string baseName, string location) => Create(baseName);

    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadResources(IWebHostEnvironment environment)
    {
        var resourcesPath = Path.Combine(environment.ContentRootPath, "Resources");
        var result = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(resourcesPath))
            return result;

        foreach (var file in Directory.GetFiles(resourcesPath, "*.json"))
        {
            var cultureName = Path.GetFileNameWithoutExtension(file);
            if (string.IsNullOrWhiteSpace(cultureName))
                continue;

            try
            {
                using var stream = File.OpenRead(file);
                using var document = JsonDocument.Parse(stream);
                var flat = new Dictionary<string, string>(StringComparer.Ordinal);
                Flatten(document.RootElement, string.Empty, flat);
                result[cultureName] = flat;
            }
            catch (JsonException)
            {
                // Ignore malformed resource files; culture simply falls back to the default.
            }
        }

        return result;
    }

    private static void Flatten(JsonElement element, string prefix, Dictionary<string, string> flat)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var key = prefix.Length == 0 ? property.Name : $"{prefix}.{property.Name}";
                    Flatten(property.Value, key, flat);
                }
                break;
            case JsonValueKind.String:
                flat[prefix] = element.GetString() ?? string.Empty;
                break;
        }
    }
}

public sealed class JsonStringLocalizer : IStringLocalizer
{
    private const string FallbackCulture = "en";

    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _resources;

    public JsonStringLocalizer(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> resources)
    {
        _resources = resources;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = Resolve(name, CultureInfo.CurrentUICulture.Name);
            return value is null
                ? new LocalizedString(name, name, resourceNotFound: true)
                : new LocalizedString(name, value);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = Resolve(name, CultureInfo.CurrentUICulture.Name);
            if (value is null)
                return new LocalizedString(name, name, resourceNotFound: true);

            return new LocalizedString(name, string.Format(value, arguments));
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        if (_resources.TryGetValue(culture, out var current))
        {
            foreach (var pair in current)
                yield return new LocalizedString(pair.Key, pair.Value);
        }
        else if (includeParentCultures && _resources.TryGetValue(FallbackCulture, out var fallback))
        {
            foreach (var pair in fallback)
                yield return new LocalizedString(pair.Key, pair.Value);
        }
    }

    private string? Resolve(string name, string cultureName)
    {
        if (_resources.TryGetValue(cultureName, out var culture) && culture.TryGetValue(name, out var value))
            return value;

        if (_resources.TryGetValue(FallbackCulture, out var fallback) && fallback.TryGetValue(name, out var fallbackValue))
            return fallbackValue;

        return null;
    }
}