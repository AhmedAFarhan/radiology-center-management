namespace RadiologyCenter.Inventory.Application.DTOs;

public record PurchaseOrderDto(
    Guid Id,
    string OrderNumber,
    Guid SupplierId,
    string SupplierName,
    string Status,
    DateTime? ExpectedDeliveryAt,
    DateTime? ReceivedAt,
    string? Notes,
    IReadOnlyList<PurchaseOrderItemDto> Items,
    string StatusKey = "");
