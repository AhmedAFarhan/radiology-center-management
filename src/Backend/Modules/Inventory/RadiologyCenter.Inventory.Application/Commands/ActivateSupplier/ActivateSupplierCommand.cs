namespace RadiologyCenter.Inventory.Application.Commands.ActivateSupplier;

public record ActivateSupplierCommand(Guid SupplierId) : ICommand;
