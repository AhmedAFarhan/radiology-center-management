using RadiologyCenter.Payroll.Application.Localization;
using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.ApprovePayRun;

public static class ApprovePayRunCommandHandler
{
    public static async Task<Result> HandleAsync(
        ApprovePayRunCommand command,
        IPayRunRepository payRunRepository,
        IPayrollUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetByIdAsync(command.PayRunId, ct);
        if (payRun is null)
            return Result.Failure(Error.NotFound(ErrorCodes.PayRunNotFound, "PayRun", command.PayRunId));

        payRun.Approve(currentUser.Name ?? currentUser.Id);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}