namespace RadiologyCenter.Inventory.Application.DTOs;

public record ItemStockDto(
    Guid ItemId,
    string ItemName,
    int StockOnHand,
    IReadOnlyList<StockBatchDto> Batches);
