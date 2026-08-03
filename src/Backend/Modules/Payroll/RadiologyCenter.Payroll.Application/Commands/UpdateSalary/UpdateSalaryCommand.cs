namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalary;

public record UpdateSalaryCommand(
    Guid SalaryId,
    decimal BaseSalary,
    string SalaryType,
    DateTime EffectiveDate) : ICommand;