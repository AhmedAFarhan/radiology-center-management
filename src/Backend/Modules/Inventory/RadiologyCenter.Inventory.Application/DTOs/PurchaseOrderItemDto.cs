namespace RadiologyCenter.Inventory.Application.DTOs;

public record PurchaseOrderItemDto(
    Guid Id,
    Guid ItemId,
    string ItemName,
    int QuantityOrdered,
    decimal UnitCost,
    int QuantityReceived);
