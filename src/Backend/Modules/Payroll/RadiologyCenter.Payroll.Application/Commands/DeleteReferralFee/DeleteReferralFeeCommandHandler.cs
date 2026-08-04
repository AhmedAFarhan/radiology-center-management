using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteReferralFee;

public static class DeleteReferralFeeCommandHandler
{
    public static Task<Result> HandleAsync(
        DeleteReferralFeeCommand command,
        IReferralFeeRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.DeleteAsync(
            repository,
            unitOfWork,
            command.Id,
            "ReferralFee",
            ct);
}