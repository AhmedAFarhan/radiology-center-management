namespace RadiologyCenter.Examinations.Application.DTOs;

public record VisitDto(
    Guid Id,
    Guid PatientId,
    Guid? AppointmentId,
    DateTime VisitedAt,
    string Status,
    string? Notes,
    IReadOnlyList<ExaminationDto> Examinations);
