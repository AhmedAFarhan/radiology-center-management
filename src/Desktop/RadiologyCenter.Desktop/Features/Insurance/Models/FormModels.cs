using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Shared.Components;

namespace RadiologyCenter.Desktop.Features.Insurance.Models;

internal sealed class CompanyFormModel
{
    [Required(ErrorMessage = "validation.nameRequired")]
    [MaxLength(200, ErrorMessage = "validation.nameMaxLength200")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "validation.taxNumberMaxLength")]
    public string? TaxId { get; set; }

    [MaxLength(30, ErrorMessage = "validation.phoneMaxLength")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "validation.emailInvalid")]
    public string? Email { get; set; }

    [MaxLength(300, ErrorMessage = "validation.addressMaxLength")]
    public string? Address { get; set; }
}

internal sealed class PolicyFormModel : IValidatableObject
{
    public string PatientId { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.policyNumberRequired")]
    [MaxLength(100, ErrorMessage = "validation.policyNumberMaxLength")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "validation.coverageRange")]
    public decimal CoveragePercent { get; set; } = 100;

    public DateTime? EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
    public bool IsGovernment { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(PatientId))
            yield return new ValidationResult("validation.patientRequired", new[] { nameof(PatientId) });

        if (string.IsNullOrWhiteSpace(CompanyId))
            yield return new ValidationResult("validation.insuranceCompanyRequired", new[] { nameof(CompanyId) });

        if (EffectiveFrom is null)
            yield return new ValidationResult("validation.effectiveFromRequired", new[] { nameof(EffectiveFrom) });

        if (EffectiveTo is not null && EffectiveFrom is not null && EffectiveTo.Value.Date < EffectiveFrom.Value.Date)
            yield return new ValidationResult(
                ValidationKeys.EffectiveToBeforeFrom,
                new[] { nameof(EffectiveTo) });
    }
}

internal sealed class PreAuthFormModel
{
    [Range(0, double.MaxValue, ErrorMessage = "validation.estimatedAmountNonNegative")]
    public decimal EstimatedAmount { get; set; }
}

internal sealed class ClaimFormModel
{
    [Range(0, double.MaxValue, ErrorMessage = "validation.billedAmountNonNegative")]
    public decimal BilledAmount { get; set; }
}
