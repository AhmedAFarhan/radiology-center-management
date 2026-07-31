using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Domain.Entities;

public sealed class StockMovement : AuditableEntity<Guid>
{
    public Guid ItemId { get; private set; }
    public Guid? StockBatchId { get; private set; }
    public StockMovementType MovementType { get; private set; }
    public int Quantity { get; private set; }
    public decimal? UnitCost { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }

    private StockMovement()
    {
        MovementType = null!;
    }

    public static StockMovement Create(
        Guid itemId,
        StockMovementType movementType,
        int quantity,
        Guid? stockBatchId = null,
        decimal? unitCost = null,
        string? reference = null,
        string? notes = null)
    {
        Guard.AgainstEmpty(itemId, nameof(itemId));
        Guard.AgainstNull(movementType, nameof(movementType));
        Guard.Against(quantity, q => q == 0, "Movement quantity cannot be zero.");

        return new StockMovement
        {
            Id = Guid.NewGuid(),
            ItemId = itemId,
            StockBatchId = stockBatchId,
            MovementType = movementType,
            Quantity = quantity,
            UnitCost = unitCost,
            Reference = reference?.Trim(),
            Notes = notes?.Trim()
        };
    }
}
