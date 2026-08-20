using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

/// <summary>
/// Loads enum options from the backend enums endpoint and caches them per culture,
/// so dropdowns can render localized values while submitting the stable enum keys.
/// </summary>
public sealed class EnumOptionsService : IDisposable
{
    private readonly ApiClient _api;
    private readonly AppLocalizer _localizer;
    private readonly Dictionary<string, IReadOnlyList<EnumOptionDto>> _cache = new(StringComparer.Ordinal);
    private bool _disposed;

    public EnumOptionsService(ApiClient api, AppLocalizer localizer)
    {
        _api = api;
        _localizer = localizer;
        _localizer.LanguageChanged += OnLanguageChanged;
    }

    public Task<IReadOnlyList<EnumOptionDto>> GetOptionsAsync(string typeName, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(typeName, out var cached))
            return Task.FromResult(cached);

        return LoadAsync(typeName, ct);
    }

    private async Task<IReadOnlyList<EnumOptionDto>> LoadAsync(string typeName, CancellationToken ct)
    {
        var options = await _api.GetAsync<IReadOnlyList<EnumOptionDto>>($"api/enums/{typeName}", ct);
        _cache[typeName] = options;
        return options;
    }

    private void OnLanguageChanged() => _cache.Clear();

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _localizer.LanguageChanged -= OnLanguageChanged;
    }
}