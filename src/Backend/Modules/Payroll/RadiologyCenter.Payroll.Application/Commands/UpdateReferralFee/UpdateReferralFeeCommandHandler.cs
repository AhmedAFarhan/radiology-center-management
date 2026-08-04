using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateReferralFee;

public static class UpdateReferralFeeCommandHandler
{
    public static Task<Result> HandleAsync(
        UpdateReferralFeeCommand command,
        IReferralFeeRepository referralFeeRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.UpdateAsync(
            referralFeeRepository,
            unitOfWork,
            command.Id,
            "ReferralFee",
            fee => fee.Update(command.Amount, command.IsPercentage),
            ct);
}