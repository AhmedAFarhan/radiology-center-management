using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaries;

public static class GetSalariesQueryHandler
{
    public static Task<Result<PagedResult<SalaryDto>>> HandleAsync(
        GetSalariesQuery query,
        ISalaryRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetPagedAsync<Salary, SalaryDto>(repository, query.Request, ct);
}