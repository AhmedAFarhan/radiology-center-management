using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteReferralFee;

public static class DeleteReferralFeeCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteReferralFeeCommand command,
        IReferralFeeRepository referralFeeRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var fee = await referralFeeRepository.GetByIdAsync(command.Id, ct);
        if (fee is null)
            return Result.Failure(Error.NotFound("ReferralFee", command.Id));

        referralFeeRepository.Remove(fee);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}