using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Identity.Domain.Events;

public sealed record UserRolesUpdatedEvent(Guid UserId, IReadOnlyCollection<Guid> RoleIds) : DomainEvent;
