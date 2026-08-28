using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Inventory.Models;

internal sealed class PurchaseOrderFormModel
{
    [Required(ErrorMessage = "Supplier is required.")]
    public string SupplierId { get; set; } = string.Empty;

    public DateTime? ExpectedDeliveryAt { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseOrderLineModel> Items { get; set; } = new();
}

internal sealed class PurchaseOrderLineModel
{
    public string ItemId { get; set; } = string.Empty;
    public int QuantityOrdered { get; set; }
    public decimal UnitCost { get; set; }
}

internal sealed class ReceiveLineModel
{
    public string Id { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int? Quantity { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}
