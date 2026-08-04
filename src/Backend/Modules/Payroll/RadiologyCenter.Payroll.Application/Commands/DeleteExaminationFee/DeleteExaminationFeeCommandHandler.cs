using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteExaminationFee;

public static class DeleteExaminationFeeCommandHandler
{
    public static Task<Result> HandleAsync(
        DeleteExaminationFeeCommand command,
        IExaminationFeeRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.DeleteAsync(
            repository,
            unitOfWork,
            command.Id,
            "ExaminationFee",
            ct);
}