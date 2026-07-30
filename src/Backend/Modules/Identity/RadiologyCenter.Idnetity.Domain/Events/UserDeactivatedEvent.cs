using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Idnetity.Domain.Events;

public sealed record UserDeactivatedEvent(Guid UserId) : DomainEvent;
