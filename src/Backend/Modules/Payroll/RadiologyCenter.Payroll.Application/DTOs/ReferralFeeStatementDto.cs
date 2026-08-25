namespace RadiologyCenter.Payroll.Application.DTOs;

public record ReferralFeeStatementDto(
    Guid Id,
    Guid ReferralDoctorId,
    decimal TotalFee,
    int ExamCount);
