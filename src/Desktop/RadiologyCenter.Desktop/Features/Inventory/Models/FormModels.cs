using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Shared.Components;

namespace RadiologyCenter.Desktop.Features.Inventory.Models;

internal sealed class ItemFormModel : IValidatableObject
{
    [Required(ErrorMessage = "validation.nameRequired")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.categoryRequired")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.unitRequired")]
    public string Unit { get; set; } = string.Empty;

    public string? Brand { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public string LotTracked { get; set; } = "No";
    public string? StorageInstructions { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ReorderLevel < 0)
            yield return new ValidationResult(ValidationKeys.ReorderLevelNonNegative, new[] { nameof(ReorderLevel) });

        if (ReorderQuantity < 0)
            yield return new ValidationResult(ValidationKeys.ReorderQuantityNonNegative, new[] { nameof(ReorderQuantity) });
    }
}

internal sealed class IssueStockModel
{
    [Range(1, int.MaxValue, ErrorMessage = "validation.quantityMinOne")]
    public int? Quantity { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }
}

internal sealed class SupplierFormModel
{
    [Required(ErrorMessage = "validation.nameRequired")]
    [MaxLength(200, ErrorMessage = "validation.nameMaxLength200")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.phoneRequired")]
    [MaxLength(30, ErrorMessage = "validation.phoneMaxLength")]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "validation.contactPersonMaxLength")]
    public string? ContactPerson { get; set; }

    [EmailAddress(ErrorMessage = "validation.emailInvalid")]
    public string? Email { get; set; }

    [MaxLength(300, ErrorMessage = "validation.addressMaxLength")]
    public string? Address { get; set; }

    [MaxLength(50, ErrorMessage = "validation.taxNumberMaxLength")]
    public string? TaxNumber { get; set; }

    [MaxLength(200, ErrorMessage = "validation.paymentTermsMaxLength")]
    public string? PaymentTerms { get; set; }
}
