using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationCompletedEvent(Guid ExaminationId) : DomainEvent;
