namespace RadiologyCenter.Examinations.Application.DTOs;

public record ExaminationDto(
    Guid Id,
    Guid VisitId,
    Guid ExaminationTypeId,
    string ExaminationTypeName,
    string ReferringDoctor,
    string ClinicalIndication,
    string Priority,
    string Status,
    DateTime? ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    Guid? PerformedByUserId,
    string? Notes,
    string? CancellationReason,
    IReadOnlyList<ExaminationItemDto> Items);
