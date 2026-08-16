using RadiologyCenter.Payroll.Application.Localization;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.DeletePayRun;

public static class DeletePayRunCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeletePayRunCommand command,
        IPayRunRepository payRunRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetByIdAsync(command.PayRunId, ct);
        if (payRun is null)
            return Result.Failure(Error.NotFound(ErrorCodes.PayRunNotFound, "PayRun", command.PayRunId));

        if (payRun.Status == PayRunStatus.Approved || payRun.Status == PayRunStatus.Paid)
            return Result.Failure(Error.Conflict(ErrorCodes.PayRunCannotDelete, $"Pay run '{command.PayRunId}' is {payRun.Status.Name} and cannot be deleted."));

        payRunRepository.Remove(payRun);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}