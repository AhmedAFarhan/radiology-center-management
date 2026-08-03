namespace RadiologyCenter.Payroll.Application.DTOs;

public record PayslipComponentDto(
    Guid Id,
    string Name,
    decimal Amount,
    bool IsDeduction);