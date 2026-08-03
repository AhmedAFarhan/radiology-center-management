namespace RadiologyCenter.Payroll.Application.DTOs;

public record ExaminationFeeDto(
    Guid Id,
    Guid ExaminationTypeId,
    string Role,
    decimal Amount,
    bool IsPercentage,
    bool IsActive);