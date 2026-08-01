using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationScheduledEvent(Guid ExaminationId, DateTime ScheduledAt) : DomainEvent;
