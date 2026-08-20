using System.Text.Json;

namespace RadiologyCenter.Desktop.Services;

public sealed class JwtClaims
{
    private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> _values;

    public JwtClaims(IReadOnlyDictionary<string, IReadOnlyList<string>> values)
    {
        _values = values;
    }

    public string? Get(string key)
        => _values.TryGetValue(key, out var values) && values.Count > 0 ? values[^1] : null;

    public IReadOnlyList<string> GetAll(string key)
        => _values.TryGetValue(key, out var values) ? values : Array.Empty<string>();
}

public static class JwtClaimsParser
{
    public static JwtClaims Parse(string? accessToken)
    {
        var empty = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(accessToken))
            return new JwtClaims(empty);

        var parts = accessToken.Split('.');
        if (parts.Length < 2)
            return new JwtClaims(empty);

        try
        {
            using var doc = JsonDocument.Parse(Base64UrlDecode(parts[1]));
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return new JwtClaims(empty);

            var values = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                    Add(values, prop.Name, prop.Value.GetString() ?? string.Empty);
            }

            return new JwtClaims(values);
        }
        catch
        {
            return new JwtClaims(empty);
        }
    }

    private static void Add(Dictionary<string, IReadOnlyList<string>> values, string key, string value)
    {
        if (values.TryGetValue(key, out var existing))
        {
            var list = existing as List<string> ?? new List<string>(existing);
            list.Add(value);
            values[key] = list;
        }
        else
        {
            values[key] = new List<string> { value };
        }
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        var padding = s.Length % 4;
        if (padding == 2)
            s += "==";
        else if (padding == 3)
            s += "=";
        return Convert.FromBase64String(s);
    }
}