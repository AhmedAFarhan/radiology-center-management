using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Patients.Models;

internal sealed class PatientFormModel : IValidatableObject
{
    [Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required.")]
    public string Gender { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
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
                "Either date of birth or age must be provided.",
                new[] { nameof(DateOfBirth) });

        if (DateOfBirth is not null && DateOfBirth.Value.Date > DateTime.UtcNow.Date)
            yield return new ValidationResult(
                "Date of birth cannot be in the future.",
                new[] { nameof(DateOfBirth) });

        if (Age is not null && Age.Value is < 0 or > 150)
            yield return new ValidationResult(
                "Age must be between 0 and 150.",
                new[] { nameof(Age) });

        var parts = FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts is null || parts.Length < 2)
            yield return new ValidationResult(
                "Full name must contain at least a first name and a last name.",
                new[] { nameof(FullName) });
    }
}
