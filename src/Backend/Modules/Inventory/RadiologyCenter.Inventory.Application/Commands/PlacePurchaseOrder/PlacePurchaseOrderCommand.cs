namespace RadiologyCenter.Inventory.Application.Commands.PlacePurchaseOrder;

public record PlacePurchaseOrderCommand(Guid PurchaseOrderId) : ICommand;
