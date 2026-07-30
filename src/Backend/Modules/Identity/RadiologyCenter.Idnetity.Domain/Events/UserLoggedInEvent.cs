using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Idnetity.Domain.Events;

public sealed record UserLoggedInEvent(Guid UserId) : DomainEvent;
