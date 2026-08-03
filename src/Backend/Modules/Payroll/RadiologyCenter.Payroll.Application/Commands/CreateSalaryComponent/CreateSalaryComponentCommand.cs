namespace RadiologyCenter.Payroll.Application.Commands.CreateSalaryComponent;

public record CreateSalaryComponentCommand(
    string Name,
    string Kind,
    bool IsPercentage = false,
    decimal DefaultValue = 0,
    string? Frequency = null,
    bool IsPerWorkDay = false) : ICommand;