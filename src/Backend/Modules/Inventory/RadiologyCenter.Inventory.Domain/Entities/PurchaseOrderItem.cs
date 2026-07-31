using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Inventory.Domain.Entities;

public sealed class PurchaseOrderItem : Entity<Guid>
{
    public Guid PurchaseOrderId { get; private set; }
    public Guid ItemId { get; private set; }
    public int QuantityOrdered { get; private set; }
    public decimal UnitCost { get; private set; }
    public int QuantityReceived { get; private set; }

    private PurchaseOrderItem() { }

    public static PurchaseOrderItem Create(
        Guid purchaseOrderId,
        Guid itemId,
        int quantityOrdered,
        decimal unitCost)
    {
        Guard.AgainstEmpty(purchaseOrderId, nameof(purchaseOrderId));
        Guard.AgainstEmpty(itemId, nameof(itemId));
        Guard.AgainstNegativeOrZero(quantityOrdered, nameof(quantityOrdered));
        Guard.Against(unitCost, c => c < 0, "Unit cost cannot be negative.");

        return new PurchaseOrderItem
        {
            Id = Guid.NewGuid(),
            PurchaseOrderId = purchaseOrderId,
            ItemId = itemId,
            QuantityOrdered = quantityOrdered,
            UnitCost = unitCost,
            QuantityReceived = 0
        };
    }

    public void RecordReceipt(int quantity)
    {
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
        Guard.Against(quantity, q => q > QuantityOrdered - QuantityReceived, $"Receipt quantity exceeds the remaining {QuantityOrdered - QuantityReceived} for the item.");

        QuantityReceived += quantity;
    }
}
