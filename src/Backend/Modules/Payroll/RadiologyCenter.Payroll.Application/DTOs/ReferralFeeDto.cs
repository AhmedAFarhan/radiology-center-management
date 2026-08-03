namespace RadiologyCenter.Payroll.Application.DTOs;

public record ReferralFeeDto(
    Guid Id,
    Guid ReferralDoctorId,
    Guid ExaminationTypeId,
    decimal Amount,
    bool IsPercentage,
    bool IsActive);