using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Inventory.Domain.Events;

public sealed record ItemCreatedEvent(Guid ItemId, string ItemName) : DomainEvent;
