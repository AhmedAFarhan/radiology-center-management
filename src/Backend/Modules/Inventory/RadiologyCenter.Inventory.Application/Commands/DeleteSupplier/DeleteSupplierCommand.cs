namespace RadiologyCenter.Inventory.Application.Commands.DeleteSupplier;

public record DeleteSupplierCommand(Guid SupplierId) : ICommand;
