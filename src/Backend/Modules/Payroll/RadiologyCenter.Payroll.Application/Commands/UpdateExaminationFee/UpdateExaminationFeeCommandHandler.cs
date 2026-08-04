using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateExaminationFee;

public static class UpdateExaminationFeeCommandHandler
{
    public static Task<Result> HandleAsync(
        UpdateExaminationFeeCommand command,
        IExaminationFeeRepository examinationFeeRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.UpdateAsync(
            examinationFeeRepository,
            unitOfWork,
            command.ExaminationFeeId,
            "ExaminationFee",
            fee => fee.Update(
                ExamFeeRole.FromName<ExamFeeRole>(command.Role),
                command.Amount,
                command.IsPercentage),
            ct);
}