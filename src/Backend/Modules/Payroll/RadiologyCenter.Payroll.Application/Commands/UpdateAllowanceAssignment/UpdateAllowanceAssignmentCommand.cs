namespace RadiologyCenter.Payroll.Application.Commands.UpdateAllowanceAssignment;

public record UpdateAllowanceAssignmentCommand(
    Guid AllowanceAssignmentId,
    string Name,
    decimal Amount,
    DateTime EffectiveDate,
    string? Frequency = null,
    DateTime? EndDate = null,
    bool IsPerWorkDay = false) : ICommand;