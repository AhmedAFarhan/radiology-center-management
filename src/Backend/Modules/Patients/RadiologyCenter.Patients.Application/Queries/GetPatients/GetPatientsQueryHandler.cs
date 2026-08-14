using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Application.DTOs;

namespace RadiologyCenter.Patients.Application.Queries.GetPatients;

public static class GetPatientsQueryHandler
{
    public static async Task<Result<PagedResult<PatientDto>>> HandleAsync(
        GetPatientsQuery query,
        IPatientRepository patientRepository,
        CancellationToken ct)
    {
        var request = WithIsActiveFilter(query.Request, query.IsActive);
        var paged = await patientRepository.GetPagedAsync(request, ct);
        var dtos = paged.Items.Select(p => p.Adapt<PatientDto>()).ToList();

        return Result.Success(new PagedResult<PatientDto>(
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
