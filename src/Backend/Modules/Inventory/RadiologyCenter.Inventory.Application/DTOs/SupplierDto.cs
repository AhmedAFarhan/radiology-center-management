namespace RadiologyCenter.Inventory.Application.DTOs;

public record SupplierDto(
    Guid Id,
    string Name,
    string? ContactPerson,
    string Phone,
    string? Email,
    string? Address,
    string? TaxNumber,
    string? PaymentTerms,
    bool IsActive,
    DateTime CreatedAt);
