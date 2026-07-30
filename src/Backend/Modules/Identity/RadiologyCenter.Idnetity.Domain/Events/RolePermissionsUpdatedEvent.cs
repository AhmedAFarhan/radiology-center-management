using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Idnetity.Domain.Events;

public sealed record RolePermissionsUpdatedEvent(Guid RoleId, IReadOnlyCollection<string> PermissionCodes) : DomainEvent;
