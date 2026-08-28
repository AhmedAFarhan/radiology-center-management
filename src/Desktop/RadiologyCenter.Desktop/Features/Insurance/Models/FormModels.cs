using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Insurance.Models;

internal sealed class CompanyFormModel
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "Tax number must be 50 characters or fewer.")]
    public string? TaxId { get; set; }

    [MaxLength(30, ErrorMessage = "Phone must be 30 characters or fewer.")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string? Email { get; set; }

    [MaxLength(300, ErrorMessage = "Address must be 300 characters or fewer.")]
    public string? Address { get; set; }
}

internal sealed class PolicyFormModel : IValidatableObject
{
    public string PatientId { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Policy number is required.")]
    [MaxLength(100, ErrorMessage = "Policy number must be 100 characters or fewer.")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Coverage must be between 0 and 100.")]
    public decimal CoveragePercent { get; set; } = 100;

    public DateTime? EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
    public bool IsGovernment { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(PatientId))
            yield return new ValidationResult("Patient is required.", new[] { nameof(PatientId) });

        if (string.IsNullOrWhiteSpace(CompanyId))
            yield return new ValidationResult("Insurance company is required.", new[] { nameof(CompanyId) });

        if (EffectiveFrom is null)
            yield return new ValidationResult("Effective From is required.", new[] { nameof(EffectiveFrom) });

        if (EffectiveTo is not null && EffectiveFrom is not null && EffectiveTo.Value.Date < EffectiveFrom.Value.Date)
            yield return new ValidationResult(
                "Effective To cannot be before Effective From.",
                new[] { nameof(EffectiveTo) });
    }
}

internal sealed class PreAuthFormModel
{
    [Range(0, double.MaxValue, ErrorMessage = "Estimated amount must be zero or greater.")]
    public decimal EstimatedAmount { get; set; }
}

internal sealed class ClaimFormModel
{
    [Range(0, double.MaxValue, ErrorMessage = "Billed amount must be zero or greater.")]
    public decimal BilledAmount { get; set; }
}
