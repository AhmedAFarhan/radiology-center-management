using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Inventory.Domain.Events;

public sealed record PurchaseOrderCreatedEvent(Guid PurchaseOrderId, string OrderNumber) : DomainEvent;
