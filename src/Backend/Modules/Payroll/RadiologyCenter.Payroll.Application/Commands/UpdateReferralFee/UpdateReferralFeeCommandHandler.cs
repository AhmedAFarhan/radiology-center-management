using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateReferralFee;

public static class UpdateReferralFeeCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateReferralFeeCommand command,
        IReferralFeeRepository referralFeeRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var fee = await referralFeeRepository.GetByIdAsync(command.Id, ct);
        if (fee is null)
            return Result.Failure(Error.NotFound("ReferralFee", command.Id));

        fee.Update(command.Amount, command.IsPercentage);

        referralFeeRepository.Update(fee);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}