using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using RadiologyCenter.BuildingBlocks.Application.Localization;

namespace RadiologyCenter.Localhost.Localization;

/// <summary>
/// Translates backend error codes and enum display names from the JSON resource files
/// (Resources\en.json, Resources\ar.json, ...). Looks up by semantic error code,
/// supports {0}/{1} placeholders, and falls back to English then the original input.
/// </summary>
public sealed class JsonTranslator : ITranslator
{
    private const string FallbackCulture = "en";

    private readonly IReadOnlyDictionary<string, CultureData> _cultures;

    public JsonTranslator(IWebHostEnvironment environment)
    {
        _cultures = Load(environment);
    }

    public string TranslateCode(string code, string? fallbackMessage = null)
    {
        if (string.IsNullOrEmpty(code))
            return fallbackMessage ?? code;

        var culture = ResolveCulture();
        if (culture is not null && culture.Codes.TryGetValue(code, out var localized))
        {
            if (localized.Contains('{') && !string.IsNullOrEmpty(fallbackMessage))
                return FormatTemplateFromEnglish(code, localized, fallbackMessage);

            return localized;
        }

        // Fallback: try English if current culture is not English
        if (!string.Equals(CultureInfo.CurrentUICulture.Name, FallbackCulture, StringComparison.OrdinalIgnoreCase) &&
            _cultures.TryGetValue(FallbackCulture, out var english) &&
            english.Codes.TryGetValue(code, out var englishValue))
        {
            if (englishValue.Contains('{') && !string.IsNullOrEmpty(fallbackMessage))
                return FormatTemplateFromEnglish(code, englishValue, fallbackMessage);

            return englishValue;
        }

        if (!string.IsNullOrEmpty(fallbackMessage))
            return fallbackMessage;

        return code;
    }

    public string TranslateEnum(string typeName, string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var culture = ResolveCulture();
        if (culture is null ||
            !culture.Enums.TryGetValue(typeName, out var values) ||
            !values.TryGetValue(name, out var localized))
        {
            return name;
        }

        return localized;
    }

    private CultureData? ResolveCulture()
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        return _cultures.TryGetValue(culture, out var current)
            ? current
            : _cultures.TryGetValue(FallbackCulture, out var fallback) ? fallback : null;
    }

    private string FormatTemplateFromEnglish(string code, string localizedTemplate, string fallbackMessage)
    {
        if (_cultures.TryGetValue(FallbackCulture, out var english)
            && english.Codes.TryGetValue(code, out var englishTemplate)
            && TryBuildTemplateRegex(englishTemplate, out var pattern, out var argCount))
        {
            var match = Regex.Match(fallbackMessage, pattern);
            if (match.Success)
            {
                var args = new object[argCount];
                for (var i = 0; i < argCount; i++)
                    args[i] = match.Groups[i + 1].Value;

                try
                {
                    return string.Format(localizedTemplate, args);
                }
                catch (FormatException)
                {
                    return fallbackMessage;
                }
            }
        }

        return fallbackMessage;
    }

    private static IReadOnlyDictionary<string, CultureData> Load(IWebHostEnvironment environment)
    {
        var directories = new List<string>();
        var sourcePath = Path.Combine(environment.ContentRootPath, "Resources");
        var outputPath = Path.Combine(AppContext.BaseDirectory, "Resources");

        if (Directory.Exists(sourcePath))
            directories.Add(sourcePath);
        if (!string.Equals(sourcePath, outputPath, StringComparison.OrdinalIgnoreCase) && Directory.Exists(outputPath))
            directories.Add(outputPath);

        var result = new Dictionary<string, CultureData>(StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var directory in directories)
        {
            foreach (var file in Directory.GetFiles(directory, "*.json"))
            {
                if (!seen.Add(file))
                    continue;

                var cultureName = ExtractCulture(file);
                if (string.IsNullOrWhiteSpace(cultureName))
                    continue;

                var parsed = TryParse(file);
                if (parsed is null)
                    continue;

                if (result.TryGetValue(cultureName, out var existing))
                    result[cultureName] = existing.Merge(parsed);
                else
                    result[cultureName] = parsed;
            }
        }

        return result;
    }

    private static string ExtractCulture(string filePath)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);
        var dot = name.LastIndexOf('.');
        return dot > 0 ? name.Substring(dot + 1) : name;
    }

    private static CultureData? TryParse(string filePath)
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            using var document = JsonDocument.Parse(stream);

            var enums = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal);
            var codes = new Dictionary<string, string>(StringComparer.Ordinal);

            if (document.RootElement.TryGetProperty("codes", out var codesElement) &&
                codesElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in codesElement.EnumerateObject())
                {
                    if (property.Value.ValueKind != JsonValueKind.String)
                        continue;

                    codes[property.Name] = property.Value.GetString() ?? property.Name;
                }
            }

            if (document.RootElement.TryGetProperty("enums", out var enumsElement) &&
                enumsElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var typeProperty in enumsElement.EnumerateObject())
                {
                    if (typeProperty.Value.ValueKind != JsonValueKind.Object)
                        continue;

                    var values = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var nameProperty in typeProperty.Value.EnumerateObject())
                    {
                        if (nameProperty.Value.ValueKind == JsonValueKind.String)
                            values[nameProperty.Name] = nameProperty.Value.GetString() ?? nameProperty.Name;
                    }

                    if (values.Count > 0)
                        enums[typeProperty.Name] = values;
                }
            }

            return new CultureData(enums, codes);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool TryBuildTemplateRegex(string template, out string pattern, out int argCount)
    {
        if (!template.Contains('{'))
        {
            pattern = string.Empty;
            argCount = 0;
            return false;
        }

        var builder = new StringBuilder("^");
        argCount = 0;
        var i = 0;
        while (i < template.Length)
        {
            if (template[i] == '{')
            {
                var close = template.IndexOf('}', i);
                if (close > i && int.TryParse(template.Substring(i + 1, close - i - 1), out var index))
                {
                    builder.Append("(.*?)");
                    argCount = Math.Max(argCount, index + 1);
                    i = close + 1;
                    continue;
                }
            }

            builder.Append(Regex.Escape(template[i].ToString()));
            i++;
        }

        builder.Append('$');
        pattern = builder.ToString();
        return true;
    }

    private sealed record CultureData(
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Enums,
        IReadOnlyDictionary<string, string> Codes)
    {
        public CultureData Merge(CultureData other)
        {
            var enums = new Dictionary<string, IReadOnlyDictionary<string, string>>(Enums, StringComparer.Ordinal);
            foreach (var kvp in other.Enums)
                enums[kvp.Key] = kvp.Value;

            var codes = new Dictionary<string, string>(Codes, StringComparer.Ordinal);
            foreach (var kvp in other.Codes)
                codes[kvp.Key] = kvp.Value;

            return new CultureData(enums, codes);
        }
    }
}
