using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponents;

public static class GetSalaryComponentsQueryHandler
{
    public static async Task<Result<PagedResult<SalaryComponentDto>>> HandleAsync(
        GetSalaryComponentsQuery query,
        ISalaryComponentRepository repository,
        CancellationToken ct)
    {
        var paged = await repository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(e => e.Adapt<SalaryComponentDto>()).ToList();

        return Result.Success(new PagedResult<SalaryComponentDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}