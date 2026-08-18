namespace RadiologyCenter.Reports.Application.DTOs;

public record ReportListItemDto(
    Guid Id,
    Guid ExaminationId,
    Guid PatientId,
    Guid RadiologistId,
    string Status,
    string StatusKey,
    int CurrentVersionNumber,
    DateTime? FinalizedAt,
    string? CancelReason,
    string? PatientName = null,
    string? RadiologistName = null,
    string? ExaminationTypeName = null);
