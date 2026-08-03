namespace RadiologyCenter.Payroll.Application.Commands.CreateSalary;

public record CreateSalaryCommand(
    Guid StaffId,
    decimal BaseSalary,
    string SalaryType,
    DateTime EffectiveDate) : ICommand;