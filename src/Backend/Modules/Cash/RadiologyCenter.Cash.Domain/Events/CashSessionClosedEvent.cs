using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Cash.Domain.Events;

public sealed record CashSessionClosedEvent(Guid CashSessionId, Guid UserId, DateTime ClosedAt) : DomainEvent;
