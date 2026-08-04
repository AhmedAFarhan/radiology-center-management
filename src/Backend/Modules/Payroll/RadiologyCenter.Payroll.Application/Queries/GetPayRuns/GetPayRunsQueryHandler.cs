using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Payroll.Application.Queries.GetPayRuns;

public static class GetPayRunsQueryHandler
{
    public static Task<Result<PagedResult<PayRunDto>>> HandleAsync(
        GetPayRunsQuery query,
        IPayRunRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetPagedAsync<PayRun, PayRunDto>(repository, query.Request, ct);
}