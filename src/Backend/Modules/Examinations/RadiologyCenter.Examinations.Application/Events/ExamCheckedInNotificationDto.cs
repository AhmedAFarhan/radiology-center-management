namespace RadiologyCenter.Examinations.Application.Events;

public sealed record ExamCheckedInNotificationDto(
    string ExaminationId,
    string PatientId,
    string PatientName,
    string PatientCode,
    string ExamName,
    string StatusKey,
    DateTime? ScheduledAt,
    string Priority,
    string PriorityKey,
    string? Indication,
    string? RadiologistId);
