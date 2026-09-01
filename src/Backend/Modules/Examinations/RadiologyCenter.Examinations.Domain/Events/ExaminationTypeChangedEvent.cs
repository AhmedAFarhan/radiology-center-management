using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationTypeChangedEvent(
    Guid ExaminationId,
    Guid OldExaminationTypeId,
    Guid NewExaminationTypeId,
    int ItemsCleared) : DomainEvent;
