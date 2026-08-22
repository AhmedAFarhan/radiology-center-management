using System.Collections.Concurrent;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

/// <summary>
/// Loads enum options from the backend enums endpoint and caches them per culture,
/// so dropdowns can render localized values while submitting the stable enum keys.
/// Concurrent callers for the same type share a single in-flight request; failed
/// loads are evicted so the next call retries.
/// </summary>
public sealed class EnumOptionsService : IDisposable
{
    private readonly ApiClient _api;
    private readonly AppLocalizer _localizer;
    private readonly ConcurrentDictionary<string, Lazy<Task<IReadOnlyList<EnumOptionDto>>>> _cache = new(StringComparer.Ordinal);
    private bool _disposed;

    public EnumOptionsService(ApiClient api, AppLocalizer localizer)
    {
        _api = api;
        _localizer = localizer;
        _localizer.LanguageChanged += OnLanguageChanged;
    }

    public async Task<IReadOnlyList<EnumOptionDto>> GetOptionsAsync(string typeName, CancellationToken ct = default)
    {
        var entry = _cache.GetOrAdd(typeName, CreateEntry);

        try
        {
            return await entry.Value.WaitAsync(ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            _cache.TryRemove(new KeyValuePair<string, Lazy<Task<IReadOnlyList<EnumOptionDto>>>>(typeName, entry));
            throw;
        }
    }

    private Lazy<Task<IReadOnlyList<EnumOptionDto>>> CreateEntry(string typeName) =>
        new(() => _api.GetAsync<IReadOnlyList<EnumOptionDto>>($"api/enums/{typeName}"));

    private void OnLanguageChanged() => _cache.Clear();

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _localizer.LanguageChanged -= OnLanguageChanged;
    }
}
