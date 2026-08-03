namespace RadiologyCenter.Payroll.Application.Commands.RemovePayslip;

public record RemovePayslipCommand(
    Guid PayRunId,
    Guid StaffId) : ICommand;