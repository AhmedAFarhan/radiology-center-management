using RadiologyCenter.BuildingBlocks.Domain.Events;
using RadiologyCenter.Examinations.Domain.Common;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationCheckedInEvent(
    Guid ExaminationId,
    Guid PatientId,
    Guid ExaminationTypeId,
    ExaminationPriority Priority,
    DateTime? ScheduledAt,
    string? ClinicalIndication,
    Guid? RadiologistId,
    Guid? TechnicianId) : DomainEvent;
