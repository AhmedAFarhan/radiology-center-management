using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Localhost.Services.GlobalSearch;

namespace RadiologyCenter.Localhost.Controllers.Search;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private const int DefaultLimit = 5;

    private readonly GlobalSearchService _search;

    public SearchController(GlobalSearchService search) => _search = search;

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] string? q, [FromQuery] int limit = DefaultLimit, CancellationToken ct = default)
    {
        var groups = await _search.SearchAsync(User, q, limit, ct);
        return Result.Success<IReadOnlyList<GlobalSearchGroupDto>>(groups).ToActionResult();
    }
}
