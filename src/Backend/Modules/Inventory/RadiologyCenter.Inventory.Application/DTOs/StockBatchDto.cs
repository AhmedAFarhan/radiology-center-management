namespace RadiologyCenter.Inventory.Application.DTOs;

public record StockBatchDto(
    Guid Id,
    Guid ItemId,
    string LotNumber,
    DateTime? ExpiryDate,
    int QuantityReceived,
    int QuantityRemaining,
    Guid? SupplierId,
    bool IsExpired);
