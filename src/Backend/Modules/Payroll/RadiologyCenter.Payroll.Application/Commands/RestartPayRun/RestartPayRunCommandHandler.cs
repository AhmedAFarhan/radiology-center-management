using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.RestartPayRun;

public static class RestartPayRunCommandHandler
{
    public static async Task<Result> HandleAsync(
        RestartPayRunCommand command,
        IPayRunRepository payRunRepository,
        IPayrollUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetByIdAsync(command.PayRunId, ct);
        if (payRun is null)
            return Result.Failure(Error.NotFound("PayRun", command.PayRunId));

        payRun.Restart(currentUser.Name ?? currentUser.Id);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}