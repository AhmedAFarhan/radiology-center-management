namespace RadiologyCenter.Payroll.Application.Commands.CreateAllowanceAssignment;

public record CreateAllowanceAssignmentCommand(
    Guid StaffId,
    string Name,
    decimal Amount,
    DateTime EffectiveDate,
    Guid? SalaryComponentId = null,
    string? Frequency = null,
    DateTime? EndDate = null,
    bool IsPerWorkDay = false) : ICommand;