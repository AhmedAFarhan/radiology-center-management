namespace RadiologyCenter.Reports.Application.DTOs;

public record ReportDto(
    Guid Id,
    Guid ExaminationId,
    Guid PatientId,
    Guid RadiologistId,
    string Status,
    string StatusKey,
    int CurrentVersionNumber,
    DateTime? FinalizedAt,
    string? CancelReason,
    ReportVersionDto CurrentVersion,
    string? PatientName = null,
    string? RadiologistName = null,
    string? ExaminationTypeName = null);
