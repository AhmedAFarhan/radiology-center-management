using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Identity.Domain.Events;

public sealed record UserRegisteredEvent(Guid UserId, string UserName, string Email) : DomainEvent;
