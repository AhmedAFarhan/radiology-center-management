using RadiologyCenter.BuildingBlocks.Domain.Events;

namespace RadiologyCenter.Inventory.Domain.Events;

public sealed record PurchaseOrderReceivedEvent(Guid PurchaseOrderId, string OrderNumber) : DomainEvent;
