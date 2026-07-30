using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Idnetity.Domain.Events;

public sealed record UserRegisteredEvent(Guid UserId, string UserName, string Email) : DomainEvent;
