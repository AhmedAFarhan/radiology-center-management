using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFees;

public static class GetExaminationFeesQueryHandler
{
    public static Task<Result<PagedResult<ExaminationFeeDto>>> HandleAsync(
        GetExaminationFeesQuery query,
        IExaminationFeeRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetPagedAsync<ExaminationFee, ExaminationFeeDto>(repository, query.Request, ct);
}