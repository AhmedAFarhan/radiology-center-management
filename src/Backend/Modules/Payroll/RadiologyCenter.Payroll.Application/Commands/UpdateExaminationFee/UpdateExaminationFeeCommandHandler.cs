using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateExaminationFee;

public static class UpdateExaminationFeeCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationFeeCommand command,
        IExaminationFeeRepository examinationFeeRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var fee = await examinationFeeRepository.GetByIdAsync(command.ExaminationFeeId, ct);
        if (fee is null)
            return Result.Failure(Error.NotFound("ExaminationFee", command.ExaminationFeeId));

        var role = ExamFeeRole.FromName<ExamFeeRole>(command.Role);
        fee.Update(role, command.Amount, command.IsPercentage);

        examinationFeeRepository.Update(fee);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}