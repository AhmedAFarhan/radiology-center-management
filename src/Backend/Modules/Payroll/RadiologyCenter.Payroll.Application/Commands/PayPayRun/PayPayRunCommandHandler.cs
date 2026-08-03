using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.PayPayRun;

public static class PayPayRunCommandHandler
{
    public static async Task<Result> HandleAsync(
        PayPayRunCommand command,
        IPayRunRepository payRunRepository,
        IPayrollUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetByIdAsync(command.PayRunId, ct);
        if (payRun is null)
            return Result.Failure(Error.NotFound("PayRun", command.PayRunId));

        payRun.Pay(currentUser.Name ?? currentUser.Id);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}