using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateReferralFee;

public static class DeactivateReferralFeeCommandHandler
{
    public static Task<Result> HandleAsync(
        DeactivateReferralFeeCommand command,
        IReferralFeeRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            repository,
            unitOfWork,
            command.Id,
            "ReferralFee",
            fee => fee.Deactivate(),
            ct);
}