namespace RadiologyCenter.Inventory.Application.Commands.UpdateSupplier;

public record UpdateSupplierCommand(
    Guid SupplierId,
    string Name,
    string Phone,
    string? ContactPerson = null,
    string? Email = null,
    string? Address = null,
    string? TaxNumber = null,
    string? PaymentTerms = null) : ICommand;
