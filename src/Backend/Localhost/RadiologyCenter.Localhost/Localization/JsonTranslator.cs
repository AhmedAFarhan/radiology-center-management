using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using RadiologyCenter.BuildingBlocks.Application.Localization;

namespace RadiologyCenter.Localhost.Localization;

/// <summary>
/// Translates backend messages and enum display names from the JSON resource files
/// (Resources\en.json, Resources\ar.json, ...). Looks up by the English message text as key,
/// supports {0}/{1} placeholders, and falls back to the English resource then the original input.
/// </summary>
public sealed class JsonTranslator : ITranslator
{
    private const string FallbackCulture = "en";

    private readonly IReadOnlyDictionary<string, CultureMessages> _cultures;

    public JsonTranslator(IWebHostEnvironment environment)
    {
        _cultures = Load(environment);
    }

    public string TranslateMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;

        var messages = ResolveCulture();
        if (messages is null)
            return message;

        if (messages.Messages.TryGetValue(message, out var exact))
            return exact;

        foreach (var template in messages.Templates)
        {
            var match = template.Regex.Match(message);
            if (!match.Success)
                continue;

            var args = new object[template.ArgCount];
            for (var i = 0; i < template.ArgCount; i++)
                args[i] = match.Groups[i + 1].Value;

            try
            {
                return string.Format(template.LocalizedTemplate, args);
            }
            catch (FormatException)
            {
                return message;
            }
        }

        return message;
    }

    public string TranslateCode(string? code, string? fallbackMessage = null)
    {
        if (string.IsNullOrEmpty(code))
            return fallbackMessage ?? code ?? string.Empty;

        var messages = ResolveCulture();
        if (messages is not null && messages.Codes.TryGetValue(code, out var localized))
        {
            if (localized.Contains('{') && !string.IsNullOrEmpty(fallbackMessage))
                return FormatTemplateFromEnglish(code, localized, fallbackMessage);

            return localized;
        }

        if (!string.IsNullOrEmpty(fallbackMessage))
            return TranslateMessage(fallbackMessage);

        return code;
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
                    return localizedTemplate;
                }
            }
        }

        return localizedTemplate;
    }

    public string TranslateEnum(string typeName, string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var messages = ResolveCulture();
        if (messages is null ||
            !messages.Enums.TryGetValue(typeName, out var values) ||
            !values.TryGetValue(name, out var localized))
        {
            return name;
        }

        return localized;
    }

    private CultureMessages? ResolveCulture()
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        return _cultures.TryGetValue(culture, out var current)
            ? current
            : _cultures.TryGetValue(FallbackCulture, out var fallback) ? fallback : null;
    }

    private static IReadOnlyDictionary<string, CultureMessages> Load(IWebHostEnvironment environment)
    {
        var directories = new List<string>();
        var sourcePath = Path.Combine(environment.ContentRootPath, "Resources");
        var outputPath = Path.Combine(AppContext.BaseDirectory, "Resources");

        if (Directory.Exists(sourcePath))
            directories.Add(sourcePath);
        if (!string.Equals(sourcePath, outputPath, StringComparison.OrdinalIgnoreCase) && Directory.Exists(outputPath))
            directories.Add(outputPath);

        var result = new Dictionary<string, CultureMessages>(StringComparer.OrdinalIgnoreCase);
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

        // Derive templates from the English code values so that English-shaped fallback
        // messages (e.g. NotFound "{entity} with key '{key}' not found.") can still be
        // localized even though many entity-specific codes are not JSON localization keys.
        if (result.TryGetValue(FallbackCulture, out var english))
        {
            foreach (var cultureName in result.Keys.ToArray())
            {
                var culture = result[cultureName];
                var templates = new List<TemplateEntry>(culture.Templates);
                foreach (var kvp in english.Codes)
                {
                    if (!culture.Codes.TryGetValue(kvp.Key, out var localized))
                        continue;

                    if (TryBuildTemplateRegex(kvp.Value, out var pattern, out var argCount))
                        templates.Add(new TemplateEntry(pattern, argCount, localized));
                }

                templates.Sort((a, b) => b.Pattern.Length.CompareTo(a.Pattern.Length));
                result[cultureName] = culture with { Templates = templates };
            }
        }

        return result;
    }

    /// <summary>
    /// Extracts the culture from a resource file name. Culture files may be named
    /// either "en.json"/"ar.json" (host-wide) or "{Module}.{culture}.json" (per module),
    /// in which case the culture is the part after the last dot.
    /// </summary>
    private static string ExtractCulture(string filePath)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);
        var dot = name.LastIndexOf('.');
        return dot > 0 ? name.Substring(dot + 1) : name;
    }

    private static CultureMessages? TryParse(string filePath)
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            using var document = JsonDocument.Parse(stream);

            var messages = new Dictionary<string, string>(StringComparer.Ordinal);
            var templates = new List<TemplateEntry>();
            var enums = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal);
            var codes = new Dictionary<string, string>(StringComparer.Ordinal);

            if (document.RootElement.TryGetProperty("messages", out var messagesElement) &&
                messagesElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in messagesElement.EnumerateObject())
                {
                    if (property.Value.ValueKind != JsonValueKind.String)
                        continue;

                    var english = property.Name;
                    var localized = property.Value.GetString() ?? english;

                    if (TryBuildTemplateRegex(english, out var pattern, out var argCount))
                        templates.Add(new TemplateEntry(pattern, argCount, localized));
                    else
                        messages[english] = localized;
                }

                templates.Sort((a, b) => b.Pattern.Length.CompareTo(a.Pattern.Length));
            }

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

            return new CultureMessages(messages, templates, enums, codes);
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

    private sealed record CultureMessages(
        IReadOnlyDictionary<string, string> Messages,
        IReadOnlyList<TemplateEntry> Templates,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Enums,
        IReadOnlyDictionary<string, string> Codes)
    {
        /// <summary>
        /// Combines two culture payloads (e.g. the host "en" file with a module "Cash.en" file).
        /// Later entries win for duplicate keys; templates are concatenated and re-sorted by length.
        /// </summary>
        public CultureMessages Merge(CultureMessages other)
        {
            var messages = new Dictionary<string, string>(Messages, StringComparer.Ordinal);
            foreach (var kvp in other.Messages)
                messages[kvp.Key] = kvp.Value;

            var templates = new List<TemplateEntry>(Templates);
            templates.AddRange(other.Templates);
            templates.Sort((a, b) => b.Pattern.Length.CompareTo(a.Pattern.Length));

            var enums = new Dictionary<string, IReadOnlyDictionary<string, string>>(Enums, StringComparer.Ordinal);
            foreach (var kvp in other.Enums)
                enums[kvp.Key] = kvp.Value;

            var codes = new Dictionary<string, string>(Codes, StringComparer.Ordinal);
            foreach (var kvp in other.Codes)
                codes[kvp.Key] = kvp.Value;

            return new CultureMessages(messages, templates, enums, codes);
        }
    }

    private sealed record TemplateEntry(string Pattern, int ArgCount, string LocalizedTemplate)
    {
        private Regex? _regex;
        public Regex Regex => _regex ??= new Regex(Pattern, RegexOptions.Compiled);
    }
}