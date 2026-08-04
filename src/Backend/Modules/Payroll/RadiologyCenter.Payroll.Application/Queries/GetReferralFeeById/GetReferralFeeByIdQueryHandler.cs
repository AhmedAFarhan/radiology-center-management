using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetReferralFeeById;

public static class GetReferralFeeByIdQueryHandler
{
    public static Task<Result<ReferralFeeDto>> HandleAsync(
        GetReferralFeeByIdQuery query,
        IReferralFeeRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetByIdAsync<ReferralFee, ReferralFeeDto>(
            repository,
            query.Id,
            "ReferralFee",
            ct);
}