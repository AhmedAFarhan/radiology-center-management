using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.Clinic.Services.GlobalSearch;

namespace RadiologyCenter.Clinic.Operations.Search;

public class SearchOperations : ClinicOperations
{
    private const int DefaultLimit = 5;

    private readonly GlobalSearchService _search;

    public SearchOperations(GlobalSearchService search) => _search = search;

    [ClinicOperation("GET", "search")]
    public async Task<ClinicResult> SearchAsync([FromQuery] string? q, [FromQuery] int limit = DefaultLimit, CancellationToken ct = default)
    {
        var groups = await _search.SearchAsync(User, q, limit, ct);
        return Result.Success<IReadOnlyList<GlobalSearchGroupDto>>(groups).ToActionResult();
    }
}
