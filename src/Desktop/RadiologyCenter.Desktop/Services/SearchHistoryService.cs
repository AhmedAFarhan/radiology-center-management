using System.Text.Json;

namespace RadiologyCenter.Desktop.Services;

public sealed class SearchHistoryService
{
    private const int MaxItems = 6;
    private const string KeyPrefix = "search.recent.";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public IReadOnlyList<string> Get(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Array.Empty<string>();

        try
        {
            var raw = Preferences.Default.Get(Key(username), string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
                return Array.Empty<string>();

            var items = JsonSerializer.Deserialize<List<string>>(raw, JsonOptions);
            return (IReadOnlyList<string>?)items ?? Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public void Add(string username, string term)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(term))
            return;

        var termTrimmed = term.Trim();
        var items = Get(username).ToList();
        items.RemoveAll(i => string.Equals(i, termTrimmed, StringComparison.OrdinalIgnoreCase));
        items.Insert(0, termTrimmed);

        if (items.Count > MaxItems)
            items.RemoveRange(MaxItems, items.Count - MaxItems);

        Preferences.Default.Set(Key(username), JsonSerializer.Serialize(items, JsonOptions));
    }

    private static string Key(string username) => KeyPrefix + username;
}