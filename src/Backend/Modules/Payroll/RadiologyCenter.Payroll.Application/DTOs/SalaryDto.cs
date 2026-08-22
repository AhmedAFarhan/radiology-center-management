namespace RadiologyCenter.Payroll.Application.DTOs;

public record SalaryDto(
    Guid Id,
    Guid StaffId,
    decimal BaseSalary,
    string SalaryType,
    DateTime EffectiveDate,
    bool IsActive,
    string SalaryTypeKey = "");