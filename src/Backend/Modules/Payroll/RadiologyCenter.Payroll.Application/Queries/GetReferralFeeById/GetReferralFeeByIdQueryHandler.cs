using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetReferralFeeById;

public static class GetReferralFeeByIdQueryHandler
{
    public static async Task<Result<ReferralFeeDto>> HandleAsync(
        GetReferralFeeByIdQuery query,
        IReferralFeeRepository referralFeeRepository,
        CancellationToken ct)
    {
        var fee = await referralFeeRepository.GetByIdAsync(query.Id, ct);
        if (fee is null)
            return Result.Failure<ReferralFeeDto>(Error.NotFound("ReferralFee", query.Id));

        return Result.Success(fee.Adapt<ReferralFeeDto>());
    }
}