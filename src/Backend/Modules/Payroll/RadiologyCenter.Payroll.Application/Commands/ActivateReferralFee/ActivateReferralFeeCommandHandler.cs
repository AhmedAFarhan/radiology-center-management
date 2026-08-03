using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateReferralFee;

public static class ActivateReferralFeeCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateReferralFeeCommand command,
        IReferralFeeRepository referralFeeRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var fee = await referralFeeRepository.GetByIdAsync(command.Id, ct);
        if (fee is null)
            return Result.Failure(Error.NotFound("ReferralFee", command.Id));

        fee.Activate();
        referralFeeRepository.Update(fee);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}