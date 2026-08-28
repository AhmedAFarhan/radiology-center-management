namespace RadiologyCenter.Desktop.Features.Inventory.Models;

public sealed record ItemDto(
    string Id,
    string Name,
    string? Brand,
    string Category,
    string Unit,
    int ReorderLevel,
    int ReorderQuantity,
    bool LotTracked,
    string? StorageInstructions,
    bool IsActive,
    DateTime CreatedAt,
    string CategoryKey = "",
    string UnitKey = "");

public sealed class ItemInput
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public bool LotTracked { get; set; }
    public string? StorageInstructions { get; set; }
}

public sealed record ItemStockDto(
    string ItemId,
    string ItemName,
    int StockOnHand,
    IReadOnlyList<StockBatchDto> Batches);

public sealed record StockBatchDto(
    string Id,
    string ItemId,
    string LotNumber,
    DateTime? ExpiryDate,
    int QuantityReceived,
    int QuantityRemaining,
    string? SupplierId,
    bool IsExpired);

public sealed class IssueStockInput
{
    public int Quantity { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}

public sealed record SupplierDto(
    string Id,
    string Name,
    string? ContactPerson,
    string Phone,
    string? Email,
    string? Address,
    string? TaxNumber,
    string? PaymentTerms,
    bool IsActive,
    DateTime CreatedAt);

public sealed class SupplierInput
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? PaymentTerms { get; set; }
}

public sealed record StockMovementDto(
    string Id,
    string ItemId,
    string? StockBatchId,
    string MovementType,
    int Quantity,
    decimal? UnitCost,
    string? Reference,
    string? Notes,
    DateTime CreatedAt,
    string MovementTypeKey = "");

public sealed record PurchaseOrderItemDto(
    string Id,
    string ItemId,
    string ItemName,
    int QuantityOrdered,
    decimal UnitCost,
    int QuantityReceived);

public sealed record PurchaseOrderDto(
    string Id,
    string OrderNumber,
    string SupplierId,
    string SupplierName,
    string Status,
    DateTime? ExpectedDeliveryAt,
    DateTime? ReceivedAt,
    string? Notes,
    IReadOnlyList<PurchaseOrderItemDto> Items,
    string StatusKey = "");

public sealed class PurchaseOrderLineInput
{
    public string ItemId { get; set; } = string.Empty;
    public int QuantityOrdered { get; set; }
    public decimal UnitCost { get; set; }
}

public sealed class CreatePurchaseOrderInput
{
    public string SupplierId { get; set; } = string.Empty;
    public List<PurchaseOrderLineInput> Items { get; set; } = new();
    public DateTime? ExpectedDeliveryAt { get; set; }
    public string? Notes { get; set; }
}

public sealed class ReceivePurchaseOrderLineInput
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}

public sealed class ReceivePurchaseOrderInput
{
    public List<ReceivePurchaseOrderLineInput> Lines { get; set; } = new();
}