namespace RadiologyCenter.Reports.Application.DTOs;

public record ReportDto(
    Guid Id,
    Guid ExaminationId,
    Guid PatientId,
    Guid RadiologistId,
    string Status,
    int CurrentVersionNumber,
    DateTime? FinalizedAt,
    string? CancelReason,
    ReportVersionDto CurrentVersion);
