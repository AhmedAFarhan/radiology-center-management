using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Application.DTOs;

namespace RadiologyCenter.Catalog.Application.Queries.GetExaminationTypes;

public static class GetExaminationTypesQueryHandler
{
    public static async Task<Result<PagedResult<ExaminationTypeDto>>> HandleAsync(
        GetExaminationTypesQuery query,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var request = WithIsActiveFilter(query.Request, query.IsActive);
        var paged = await examinationTypeRepository.GetPagedWithItemsAsync(request, ct);
        var dtos = paged.Items.Select(t => t.Adapt<ExaminationTypeDto>()).ToList();

        return Result.Success(new PagedResult<ExaminationTypeDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }

    private static QueryRequest WithIsActiveFilter(QueryRequest request, bool? isActive)
    {
        if (isActive is null)
            return request;

        var filters = (request.Filters ?? []).ToList();
        filters.Add(new FilterCriteria { Field = "IsActive", Operator = FilterOperator.Equals, Value = isActive.Value });

        return new QueryRequest
        {
            Pagination = request.Pagination,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            SearchTerm = request.SearchTerm,
            SearchFields = request.SearchFields,
            Filters = filters
        };
    }
}
