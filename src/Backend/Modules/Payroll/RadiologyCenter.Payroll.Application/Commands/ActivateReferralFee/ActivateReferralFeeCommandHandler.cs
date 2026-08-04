using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateReferralFee;

public static class ActivateReferralFeeCommandHandler
{
    public static Task<Result> HandleAsync(
        ActivateReferralFeeCommand command,
        IReferralFeeRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            repository,
            unitOfWork,
            command.Id,
            "ReferralFee",
            fee => fee.Activate(),
            ct);
}