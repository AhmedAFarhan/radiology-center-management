using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record ExaminationScheduledEvent(Guid VisitId, Guid ExaminationId, DateTime ScheduledAt) : DomainEvent;
