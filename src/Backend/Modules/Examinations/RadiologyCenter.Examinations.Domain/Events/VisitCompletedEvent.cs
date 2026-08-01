using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record VisitCompletedEvent(Guid VisitId) : DomainEvent;
