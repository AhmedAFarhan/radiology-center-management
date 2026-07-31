using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Inventory.Domain.Enumerations;
using RadiologyCenter.Inventory.Domain.Events;

namespace RadiologyCenter.Inventory.Domain.Entities;

public sealed class PurchaseOrder : SoftDeletableAggregateRoot<Guid>
{
    private readonly List<PurchaseOrderItem> _items = [];

    public string OrderNumber { get; private set; }
    public Guid SupplierId { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public DateTime? ExpectedDeliveryAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<PurchaseOrderItem> Items => _items.AsReadOnly();

    private PurchaseOrder()
    {
        OrderNumber = null!;
        Status = null!;
    }

    public static PurchaseOrder Create(
        string orderNumber,
        Guid supplierId,
        DateTime? expectedDeliveryAt = null,
        string? notes = null)
    {
        Guard.AgainstNullOrWhiteSpace(orderNumber, nameof(orderNumber));
        Guard.AgainstEmpty(supplierId, nameof(supplierId));

        var purchaseOrder = new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber.Trim(),
            SupplierId = supplierId,
            Status = PurchaseOrderStatus.Draft,
            ExpectedDeliveryAt = expectedDeliveryAt,
            Notes = notes?.Trim()
        };

        purchaseOrder.RaiseDomainEvent(new PurchaseOrderCreatedEvent(purchaseOrder.Id, purchaseOrder.OrderNumber));
        return purchaseOrder;
    }

    public void AddItem(Guid itemId, int quantityOrdered, decimal unitCost)
    {
        EnsureDraft();
        Guard.Against(_items.Any(i => i.ItemId == itemId), _ => true, $"Item '{itemId}' is already on purchase order '{OrderNumber}'.");

        _items.Add(PurchaseOrderItem.Create(Id, itemId, quantityOrdered, unitCost));
    }

    public void RemoveItem(Guid itemId)
    {
        EnsureDraft();

        var line = _items.FirstOrDefault(i => i.ItemId == itemId)
            ?? throw new DomainException($"Item '{itemId}' is not on purchase order '{OrderNumber}'.");
        _items.Remove(line);
    }

    public void Place()
    {
        EnsureDraft();
        Guard.Against(_items.Count, c => c == 0, $"Cannot place purchase order '{OrderNumber}' without items.");

        Status = PurchaseOrderStatus.Ordered;
    }

    public void Cancel()
    {
        if (Status != PurchaseOrderStatus.Draft && Status != PurchaseOrderStatus.Ordered)
            throw new DomainException($"Purchase order '{OrderNumber}' in status '{Status}' cannot be cancelled.");

        Status = PurchaseOrderStatus.Cancelled;
    }

    public void RecordReceipt(Guid itemId, int quantity)
    {
        if (Status != PurchaseOrderStatus.Ordered && Status != PurchaseOrderStatus.PartiallyReceived)
            throw new DomainException($"Receipts can only be recorded against an ordered purchase order, not '{Status}'.");

        var line = _items.FirstOrDefault(i => i.ItemId == itemId)
            ?? throw new DomainException($"Item '{itemId}' is not on purchase order '{OrderNumber}'.");
        line.RecordReceipt(quantity);

        if (_items.All(i => i.QuantityReceived >= i.QuantityOrdered))
        {
            Status = PurchaseOrderStatus.Received;
            ReceivedAt = DateTime.UtcNow;
            RaiseDomainEvent(new PurchaseOrderReceivedEvent(Id, OrderNumber));
        }
        else
        {
            Status = PurchaseOrderStatus.PartiallyReceived;
        }
    }

    private void EnsureDraft()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException($"Purchase order '{OrderNumber}' is no longer a draft (status: '{Status}').");
    }
}
