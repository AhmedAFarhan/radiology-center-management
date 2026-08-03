namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalaryComponent;

public record UpdateSalaryComponentCommand(
    Guid SalaryComponentId,
    string Name,
    string Kind,
    bool IsPercentage,
    decimal DefaultValue,
    string? Frequency = null,
    bool IsPerWorkDay = false) : ICommand;