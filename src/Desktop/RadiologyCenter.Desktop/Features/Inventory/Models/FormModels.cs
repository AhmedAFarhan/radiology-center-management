using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Inventory.Models;

internal sealed class ItemFormModel : IValidatableObject
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Unit is required.")]
    public string Unit { get; set; } = string.Empty;

    public string? Brand { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public string LotTracked { get; set; } = "No";
    public string? StorageInstructions { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ReorderLevel < 0)
            yield return new ValidationResult("Reorder level cannot be negative.", new[] { nameof(ReorderLevel) });

        if (ReorderQuantity < 0)
            yield return new ValidationResult("Reorder quantity cannot be negative.", new[] { nameof(ReorderQuantity) });
    }
}

internal sealed class IssueStockModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int? Quantity { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }
}

internal sealed class SupplierFormModel
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [MaxLength(30, ErrorMessage = "Phone must be 30 characters or fewer.")]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Contact person must be 100 characters or fewer.")]
    public string? ContactPerson { get; set; }

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string? Email { get; set; }

    [MaxLength(300, ErrorMessage = "Address must be 300 characters or fewer.")]
    public string? Address { get; set; }

    [MaxLength(50, ErrorMessage = "Tax number must be 50 characters or fewer.")]
    public string? TaxNumber { get; set; }

    [MaxLength(200, ErrorMessage = "Payment terms must be 200 characters or fewer.")]
    public string? PaymentTerms { get; set; }
}
