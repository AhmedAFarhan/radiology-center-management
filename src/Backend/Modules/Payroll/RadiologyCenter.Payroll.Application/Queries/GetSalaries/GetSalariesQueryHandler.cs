using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaries;

public static class GetSalariesQueryHandler
{
    public static async Task<Result<PagedResult<SalaryDto>>> HandleAsync(
        GetSalariesQuery query,
        ISalaryRepository salaryRepository,
        CancellationToken ct)
    {
        var paged = await salaryRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(s => s.Adapt<SalaryDto>()).ToList();

        return Result.Success(new PagedResult<SalaryDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}