namespace RadiologyCenter.Payroll.Application.Commands.AddPayslip;

public record AddPayslipCommand(
    Guid PayRunId,
    Guid StaffId) : ICommand;