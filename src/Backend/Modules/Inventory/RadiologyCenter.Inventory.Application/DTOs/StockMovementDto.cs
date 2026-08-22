namespace RadiologyCenter.Inventory.Application.DTOs;

public record StockMovementDto(
    Guid Id,
    Guid ItemId,
    Guid? StockBatchId,
    string MovementType,
    int Quantity,
    decimal? UnitCost,
    string? Reference,
    string? Notes,
    DateTime CreatedAt,
    string MovementTypeKey = "");
