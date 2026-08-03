using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.RemovePayslip;

public static class RemovePayslipCommandHandler
{
    public static async Task<Result> HandleAsync(
        RemovePayslipCommand command,
        IPayRunRepository payRunRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetWithPayslipsAsync(command.PayRunId, ct);
        if (payRun is null)
            return Result.Failure(Error.NotFound("PayRun", command.PayRunId));

        payRun.RemovePayslip(command.StaffId);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}