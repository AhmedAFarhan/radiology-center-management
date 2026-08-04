using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.ComputePayRun;

public static class ComputePayRunCommandHandler
{
    public static async Task<Result> HandleAsync(
        ComputePayRunCommand command,
        IPayRunRepository payRunRepository,
        IPayrollStaffDirectory payrollStaffDirectory,
        IPayslipCalculator payslipCalculator,
        IPayrollUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetWithPayslipsAsync(command.PayRunId, ct);
        if (payRun is null)
            return Result.Failure(Error.NotFound("PayRun", command.PayRunId));

        if (payRun.Status != PayRunStatus.Draft)
            return Result.Failure(Error.Conflict($"Pay run '{command.PayRunId}' is {payRun.Status.Name} and cannot be recomputed."));

        var staffIds = await payrollStaffDirectory.GetActiveStaffIdsAsync(ct);

        foreach (var staffId in staffIds)
        {
            var draft = await payslipCalculator.CalculateAsync(staffId, payRun.RunFrom, payRun.RunTo, ct);
            if (draft is null)
                continue;

            payRun.SetPayslipDraft(
                draft.StaffId,
                draft.BaseSalary,
                draft.UnpaidLeaveDays,
                draft.UnpaidLeaveDeduction,
                draft.Components.Select(c => (c.Name, c.Amount, c.IsDeduction)).ToList());
        }

        payRun.Compute(currentUser.Name ?? currentUser.Id);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}