using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Cash.Domain.Events;

public sealed record CashSessionOpenedEvent(Guid CashSessionId, Guid UserId, decimal OpeningFloat) : DomainEvent;
