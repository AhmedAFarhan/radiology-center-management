using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationStartedEvent(
    Guid ExaminationId,
    Guid PatientId,
    Guid ExaminationTypeId,
    Guid PerformedByUserId) : DomainEvent;
