using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Idnetity.Domain.Events;

public sealed record UserReactivatedEvent(Guid UserId) : DomainEvent;
