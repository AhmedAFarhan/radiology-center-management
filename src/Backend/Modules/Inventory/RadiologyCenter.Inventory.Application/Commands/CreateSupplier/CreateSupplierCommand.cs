namespace RadiologyCenter.Inventory.Application.Commands.CreateSupplier;

public record CreateSupplierCommand(
    string Name,
    string Phone,
    string? ContactPerson = null,
    string? Email = null,
    string? Address = null,
    string? TaxNumber = null,
    string? PaymentTerms = null) : ICommand;
