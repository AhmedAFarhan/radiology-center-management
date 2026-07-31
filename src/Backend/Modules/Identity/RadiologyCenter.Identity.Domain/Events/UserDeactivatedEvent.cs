using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Identity.Domain.Events;

public sealed record UserDeactivatedEvent(Guid UserId) : DomainEvent;
