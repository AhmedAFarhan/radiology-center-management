namespace RadiologyCenter.Inventory.Application.Commands.ReceivePurchaseOrder;

public record ReceivePurchaseOrderLineInput(
    Guid ItemId,
    int Quantity,
    string LotNumber,
    DateTime? ExpiryDate = null);

public record ReceivePurchaseOrderCommand(
    Guid PurchaseOrderId,
    List<ReceivePurchaseOrderLineInput> Lines) : ICommand;
