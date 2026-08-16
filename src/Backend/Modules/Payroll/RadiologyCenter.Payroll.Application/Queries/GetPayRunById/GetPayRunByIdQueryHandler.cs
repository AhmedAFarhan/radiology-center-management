using Mapster;
using RadiologyCenter.Payroll.Application.Localization;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetPayRunById;

public static class GetPayRunByIdQueryHandler
{
    public static async Task<Result<PayRunDto>> HandleAsync(
        GetPayRunByIdQuery query,
        IPayRunRepository payRunRepository,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetWithPayslipsAsync(query.Id, ct);
        if (payRun is null)
            return Result.Failure<PayRunDto>(Error.NotFound(ErrorCodes.PayRunNotFound, "PayRun", query.Id));

        return Result.Success(payRun.Adapt<PayRunDto>());
    }
}