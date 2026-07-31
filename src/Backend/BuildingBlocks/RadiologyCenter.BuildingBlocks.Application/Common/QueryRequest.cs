using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;

namespace RadiologyCenter.BuildingBlocks.Application.Common;

public class QueryRequest
{
    public PaginationParams Pagination { get; init; } = new();
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public string? SearchTerm { get; init; }
    public List<string>? SearchFields { get; init; }
    public List<FilterCriteria>? Filters { get; init; }
}
