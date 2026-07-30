using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Idnetity.Domain.Events;

public sealed record RoleCreatedEvent(Guid RoleId, string RoleName) : DomainEvent;
