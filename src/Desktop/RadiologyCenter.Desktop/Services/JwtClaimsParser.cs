using System.Text.Json;

namespace RadiologyCenter.Desktop.Services;

public static class JwtClaimsParser
{
    public static IReadOnlyDictionary<string, string> Parse(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var parts = accessToken.Split('.');
        if (parts.Length < 2)
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var doc = JsonDocument.Parse(Base64UrlDecode(parts[1]));
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                    result[prop.Name] = prop.Value.GetString() ?? string.Empty;
            }

            return result;
        }
        catch
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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