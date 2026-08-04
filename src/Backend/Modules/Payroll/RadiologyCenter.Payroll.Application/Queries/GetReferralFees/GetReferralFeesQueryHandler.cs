using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Payroll.Application.Queries.GetReferralFees;

public static class GetReferralFeesQueryHandler
{
    public static Task<Result<PagedResult<ReferralFeeDto>>> HandleAsync(
        GetReferralFeesQuery query,
        IReferralFeeRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetPagedAsync<ReferralFee, ReferralFeeDto>(repository, query.Request, ct);
}