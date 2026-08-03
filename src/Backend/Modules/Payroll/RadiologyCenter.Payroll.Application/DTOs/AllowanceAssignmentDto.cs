namespace RadiologyCenter.Payroll.Application.DTOs;

public record AllowanceAssignmentDto(
    Guid Id,
    Guid StaffId,
    Guid? SalaryComponentId,
    string Name,
    decimal Amount,
    string? Frequency,
    bool IsPerWorkDay,
    DateTime EffectiveDate,
    DateTime? EndDate,
    bool IsActive);