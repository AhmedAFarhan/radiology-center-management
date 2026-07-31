namespace RadiologyCenter.Inventory.Application.Commands.CreatePurchaseOrder;

public record PurchaseOrderLineInput(
    Guid ItemId,
    int QuantityOrdered,
    decimal UnitCost);

public record CreatePurchaseOrderCommand(
    Guid SupplierId,
    List<PurchaseOrderLineInput> Items,
    DateTime? ExpectedDeliveryAt = null,
    string? Notes = null) : ICommand;
