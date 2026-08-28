using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Resources.Models;

internal sealed class StaffFormModel : IValidatableObject
{
    [Required(ErrorMessage = "Full name is required.")]
    [MaxLength(300, ErrorMessage = "Full name must be 300 characters or fewer.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required.")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [MaxLength(30, ErrorMessage = "Phone number must be 30 characters or fewer.")]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime? HireDate { get; set; }

    [MaxLength(200, ErrorMessage = "Department must be 200 characters or fewer.")]
    public string? Department { get; set; }

    [MaxLength(200, ErrorMessage = "Specialization must be 200 characters or fewer.")]
    public string? Specialization { get; set; }

    [MaxLength(100, ErrorMessage = "License number must be 100 characters or fewer.")]
    public string? LicenseNumber { get; set; }

    public string SalaryCalculationRule { get; set; } = "FixedPlusFees";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            yield return new ValidationResult("Full name must include at least a first and last name.", new[] { nameof(FullName) });

        if (HireDate is null)
            yield return new ValidationResult("Hire date is required.", new[] { nameof(HireDate) });

        if (!string.IsNullOrWhiteSpace(PhoneNumber) && !EgyptianPhoneNumber.IsValid(PhoneNumber))
            yield return new ValidationResult("Phone number must be a valid Egyptian number (e.g. 01012345678).", new[] { nameof(PhoneNumber) });
    }
}

internal sealed class EquipmentFormModel
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Modality is required.")]
    public string Modality { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Serial number must be 100 characters or fewer.")]
    public string? SerialNumber { get; set; }

    public DateTime? PurchaseDate { get; set; }
}

internal sealed class EquipmentStatusModel
{
    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; } = string.Empty;
}

internal sealed class LeaveFormModel
{
    [Required(ErrorMessage = "Leave type is required.")]
    public string LeaveType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required.")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    public DateTime? EndDate { get; set; }

    [MaxLength(500, ErrorMessage = "Reason must be 500 characters or fewer.")]
    public string? Reason { get; set; }
}

internal sealed class WorkShiftFormModel
{
    [Required(ErrorMessage = "Shift date is required.")]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    public TimeSpan? StartTime { get; set; }

    [Required(ErrorMessage = "End time is required.")]
    public TimeSpan? EndTime { get; set; }

    [MaxLength(500, ErrorMessage = "Notes must be 500 characters or fewer.")]
    public string? Notes { get; set; }
}

internal sealed class ReferralDoctorFormModel : IValidatableObject
{
    [Required(ErrorMessage = "Full name is required.")]
    [MaxLength(300, ErrorMessage = "Full name must be 300 characters or fewer.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [MaxLength(30, ErrorMessage = "Phone number must be 30 characters or fewer.")]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Email must be 200 characters or fewer.")]
    public string? Email { get; set; }

    [MaxLength(200, ErrorMessage = "Specialization must be 200 characters or fewer.")]
    public string? Specialization { get; set; }

    [MaxLength(200, ErrorMessage = "Hospital must be 200 characters or fewer.")]
    public string? Hospital { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            yield return new ValidationResult("Full name must include at least a first and last name.", new[] { nameof(FullName) });

        if (!string.IsNullOrWhiteSpace(Phone) && !EgyptianPhoneNumber.IsValid(Phone))
            yield return new ValidationResult("Phone number must be a valid Egyptian number (e.g. 01012345678).", new[] { nameof(Phone) });
    }
}
