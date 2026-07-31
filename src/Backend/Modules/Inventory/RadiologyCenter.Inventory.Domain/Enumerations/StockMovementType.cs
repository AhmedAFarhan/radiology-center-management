using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Inventory.Domain.Enumerations;

public sealed class StockMovementType : Enumeration
{
    public static readonly StockMovementType Receive = new(1, "Receive");
    public static readonly StockMovementType Issue = new(2, "Issue");
    public static readonly StockMovementType Adjustment = new(3, "Adjustment");
    public static readonly StockMovementType ReturnToSupplier = new(4, "ReturnToSupplier");
    public static readonly StockMovementType Disposal = new(5, "Disposal");

    private StockMovementType(int value, string name) : base(value, name) { }
}
