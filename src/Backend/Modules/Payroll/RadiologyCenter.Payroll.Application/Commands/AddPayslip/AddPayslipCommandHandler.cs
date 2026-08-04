using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;
using Mapster;

namespace RadiologyCenter.Payroll.Application.Commands.AddPayslip;

public static class AddPayslipCommandHandler
{
    public static async Task<Result<PayslipDto>> HandleAsync(
        AddPayslipCommand command,
        IPayRunRepository payRunRepository,
        IPayrollStaffDirectory payrollStaffDirectory,
        IPayslipCalculator payslipCalculator,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var payRun = await payRunRepository.GetWithPayslipsAsync(command.PayRunId, ct);
        if (payRun is null)
            return Result.Failure<PayslipDto>(Error.NotFound("PayRun", command.PayRunId));

        if (!await payrollStaffDirectory.ExistsAsync(command.StaffId, ct))
            return Result.Failure<PayslipDto>(Error.NotFound("Staff", command.StaffId));

        var draft = await payslipCalculator.CalculateAsync(command.StaffId, payRun.RunFrom, payRun.RunTo, ct);
        if (draft is null)
            return Result.Failure<PayslipDto>(Error.Failure("Unable to calculate a payslip for the given staff."));

        var payslip = payRun.SetPayslipDraft(
            draft.StaffId,
            draft.BaseSalary,
            draft.UnpaidLeaveDays,
            draft.UnpaidLeaveDeduction,
            draft.Components.Select(c => (c.Name, c.Amount, c.IsDeduction)).ToList());

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(payslip.Adapt<PayslipDto>());
    }
}