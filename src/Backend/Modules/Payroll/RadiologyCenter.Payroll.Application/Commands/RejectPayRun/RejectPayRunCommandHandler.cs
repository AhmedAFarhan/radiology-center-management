using RadiologyCenter.Payroll.Application.Localization;
using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.RejectPayRun;

public static class RejectPayRunCommandHandler
{
    public static async Task<Result> HandleAsync(
        RejectPayRunCommand command,
        IPayRunRepository payRunRepository,
        IPayrollUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetByIdAsync(command.PayRunId, ct);
        if (payRun is null)
            return Result.Failure(Error.NotFound(ErrorCodes.PayRunNotFound, "PayRun", command.PayRunId));

        payRun.Reject(currentUser.Name ?? currentUser.Id);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}