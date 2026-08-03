using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteExaminationFee;

public static class DeleteExaminationFeeCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteExaminationFeeCommand command,
        IExaminationFeeRepository examinationFeeRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var fee = await examinationFeeRepository.GetByIdAsync(command.Id, ct);
        if (fee is null)
            return Result.Failure(Error.NotFound("ExaminationFee", command.Id));

        examinationFeeRepository.Remove(fee);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}