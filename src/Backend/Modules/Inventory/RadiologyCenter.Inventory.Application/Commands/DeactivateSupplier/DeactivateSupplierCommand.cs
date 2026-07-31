namespace RadiologyCenter.Inventory.Application.Commands.DeactivateSupplier;

public record DeactivateSupplierCommand(Guid SupplierId) : ICommand;
