using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateExaminationFee;

public static class ActivateExaminationFeeCommandHandler
{
    public static Task<Result> HandleAsync(
        ActivateExaminationFeeCommand command,
        IExaminationFeeRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            repository,
            unitOfWork,
            command.Id,
            "ExaminationFee",
            fee => fee.Activate(),
            ct);
}