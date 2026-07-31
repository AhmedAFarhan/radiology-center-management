using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Inventory.Domain.Enumerations;

public sealed class PurchaseOrderStatus : Enumeration
{
    public static readonly PurchaseOrderStatus Draft = new(1, "Draft");
    public static readonly PurchaseOrderStatus Ordered = new(2, "Ordered");
    public static readonly PurchaseOrderStatus PartiallyReceived = new(3, "PartiallyReceived");
    public static readonly PurchaseOrderStatus Received = new(4, "Received");
    public static readonly PurchaseOrderStatus Cancelled = new(5, "Cancelled");

    private PurchaseOrderStatus(int value, string name) : base(value, name) { }
}
