namespace RadiologyCenter.Payroll.Application.DTOs;

public record PayRunDto(
    Guid Id,
    DateTime RunFrom,
    DateTime RunTo,
    string Status,
    string? ProcessedBy,
    DateTime? ProcessedAt,
    string? Notes,
    IReadOnlyList<PayslipDto> Payslips);