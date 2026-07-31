using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Inventory.Domain.Events;

public sealed record ItemUpdatedEvent(Guid ItemId) : DomainEvent;
