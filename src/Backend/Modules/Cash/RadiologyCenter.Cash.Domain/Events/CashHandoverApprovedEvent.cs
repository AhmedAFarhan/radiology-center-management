using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Cash.Domain.Events;

public sealed record CashHandoverApprovedEvent(Guid CashSessionId, Guid HandoverId, Guid ApprovedByUserId) : DomainEvent;
