using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Identity.Domain.Events;

public sealed record RoleCreatedEvent(Guid RoleId, string RoleName) : DomainEvent;
