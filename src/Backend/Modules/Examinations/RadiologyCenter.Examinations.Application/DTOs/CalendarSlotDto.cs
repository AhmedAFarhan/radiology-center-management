namespace RadiologyCenter.Examinations.Application.DTOs;

public record CalendarSlotDto(
    Guid Id,
    Guid? EquipmentId,
    string? EquipmentName,
    Guid? RadiologistId,
    string? RadiologistName,
    string PatientName,
    string ExaminationTypeName,
    string Modality,
    DateTime ScheduledAt,
    DateTime? ScheduledEnd,
    string Status,
    string Priority);
