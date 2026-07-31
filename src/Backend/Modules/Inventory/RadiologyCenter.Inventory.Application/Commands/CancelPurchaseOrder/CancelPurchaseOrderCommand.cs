namespace RadiologyCenter.Inventory.Application.Commands.CancelPurchaseOrder;

public record CancelPurchaseOrderCommand(Guid PurchaseOrderId) : ICommand;
