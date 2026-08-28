using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Shared.Components;

namespace RadiologyCenter.Desktop.Features.Patients.Models;

internal sealed class PatientFormModel : IValidatableObject
{
    [Required(ErrorMessage = "validation.fullNameRequired")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.genderRequired")]
    public string Gender { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }

    [Required(ErrorMessage = "validation.phoneNumberRequired")]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "validation.emailInvalid")]
    public string? Email { get; set; }

    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalHistory { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth is null && Age is null)
            yield return new ValidationResult(
                ValidationKeys.DobOrAgeRequired,
                new[] { nameof(DateOfBirth) });

        if (DateOfBirth is not null && DateOfBirth.Value.Date > DateTime.UtcNow.Date)
            yield return new ValidationResult(
                ValidationKeys.DobNotFuture,
                new[] { nameof(DateOfBirth) });

        if (Age is not null && Age.Value is < 0 or > 150)
            yield return new ValidationResult(
                ValidationKeys.AgeRange,
                new[] { nameof(Age) });

        var parts = FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts is null || parts.Length < 2)
            yield return new ValidationResult(
                ValidationKeys.FullNameFirstLast,
                new[] { nameof(FullName) });
    }
}
