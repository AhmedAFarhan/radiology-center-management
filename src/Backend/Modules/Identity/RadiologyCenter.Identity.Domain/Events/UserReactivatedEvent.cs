using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Identity.Domain.Events;

public sealed record UserReactivatedEvent(Guid UserId) : DomainEvent;
