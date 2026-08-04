using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateExaminationFee;

public static class DeactivateExaminationFeeCommandHandler
{
    public static Task<Result> HandleAsync(
        DeactivateExaminationFeeCommand command,
        IExaminationFeeRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            repository,
            unitOfWork,
            command.Id,
            "ExaminationFee",
            fee => fee.Deactivate(),
            ct);
}