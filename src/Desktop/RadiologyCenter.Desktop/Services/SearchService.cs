using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class SearchService
{
    private readonly ApiClient _api;

    public SearchService(ApiClient api) => _api = api;

    public Task<IReadOnlyList<GlobalSearchGroupDto>> SearchAsync(string term, int limit = 5, CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<GlobalSearchGroupDto>>($"api/search?q={Uri.EscapeDataString(term)}&limit={limit}", ct);
}