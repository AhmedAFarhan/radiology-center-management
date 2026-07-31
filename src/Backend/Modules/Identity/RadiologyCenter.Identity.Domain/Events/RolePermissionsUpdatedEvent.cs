using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Identity.Domain.Events;

public sealed record RolePermissionsUpdatedEvent(Guid RoleId, IReadOnlyCollection<string> PermissionCodes) : DomainEvent;
