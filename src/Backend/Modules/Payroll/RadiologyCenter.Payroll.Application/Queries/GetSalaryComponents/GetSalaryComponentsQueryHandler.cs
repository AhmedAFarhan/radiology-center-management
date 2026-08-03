using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponents;

public static class GetSalaryComponentsQueryHandler
{
    public static async Task<Result<PagedResult<SalaryComponentDto>>> HandleAsync(
        GetSalaryComponentsQuery query,
        ISalaryComponentRepository salaryComponentRepository,
        CancellationToken ct)
    {
        var paged = await salaryComponentRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(c => c.Adapt<SalaryComponentDto>()).ToList();

        return Result.Success(new PagedResult<SalaryComponentDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}