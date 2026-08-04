using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponents;

public static class GetSalaryComponentsQueryHandler
{
    public static Task<Result<PagedResult<SalaryComponentDto>>> HandleAsync(
        GetSalaryComponentsQuery query,
        ISalaryComponentRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetPagedAsync<SalaryComponent, SalaryComponentDto>(repository, query.Request, ct);
}