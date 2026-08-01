using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Examinations.Domain.Events;

public sealed record VisitCancelledEvent(Guid VisitId) : DomainEvent;
