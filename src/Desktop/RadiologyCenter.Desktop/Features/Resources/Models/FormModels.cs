using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Shared.Components;

namespace RadiologyCenter.Desktop.Features.Resources.Models;

internal sealed class StaffFormModel : IValidatableObject
{
    [Required(ErrorMessage = "validation.fullNameRequired")]
    [MaxLength(300, ErrorMessage = "validation.fullNameMaxLength")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.positionRequired")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.phoneNumberRequired")]
    [MaxLength(30, ErrorMessage = "validation.phoneNumberMaxLength")]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime? HireDate { get; set; }

    [MaxLength(200, ErrorMessage = "validation.departmentMaxLength")]
    public string? Department { get; set; }

    [MaxLength(200, ErrorMessage = "validation.specializationMaxLength")]
    public string? Specialization { get; set; }

    [MaxLength(100, ErrorMessage = "validation.licenseNumberMaxLength")]
    public string? LicenseNumber { get; set; }

    public string SalaryCalculationRule { get; set; } = "FixedPlusFees";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            yield return new ValidationResult(ValidationKeys.FullNameFirstAndLast, new[] { nameof(FullName) });

        if (HireDate is null)
            yield return new ValidationResult(ValidationKeys.HireDateRequired, new[] { nameof(HireDate) });

        if (!string.IsNullOrWhiteSpace(PhoneNumber) && !EgyptianPhoneNumber.IsValid(PhoneNumber))
            yield return new ValidationResult(ValidationKeys.PhoneEgyptian, new[] { nameof(PhoneNumber) });
    }
}

internal sealed class EquipmentFormModel
{
    [Required(ErrorMessage = "validation.nameRequired")]
    [MaxLength(200, ErrorMessage = "validation.nameMaxLength200")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.modalityRequired")]
    public string Modality { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "validation.serialNumberMaxLength")]
    public string? SerialNumber { get; set; }

    public DateTime? PurchaseDate { get; set; }
}

internal sealed class EquipmentStatusModel
{
    [Required(ErrorMessage = "validation.statusRequired")]
    public string Status { get; set; } = string.Empty;
}

internal sealed class LeaveFormModel
{
    [Required(ErrorMessage = "validation.leaveTypeRequired")]
    public string LeaveType { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.startDateRequired")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "validation.endDateRequired")]
    public DateTime? EndDate { get; set; }

    [MaxLength(500, ErrorMessage = "validation.reasonMaxLength")]
    public string? Reason { get; set; }
}

internal sealed class WorkShiftFormModel
{
    [Required(ErrorMessage = "validation.shiftDateRequired")]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "validation.startTimeRequired")]
    public TimeSpan? StartTime { get; set; }

    [Required(ErrorMessage = "validation.endTimeRequired")]
    public TimeSpan? EndTime { get; set; }

    [MaxLength(500, ErrorMessage = "validation.notesMaxLength500")]
    public string? Notes { get; set; }
}

internal sealed class ReferralDoctorFormModel : IValidatableObject
{
    [Required(ErrorMessage = "validation.fullNameRequired")]
    [MaxLength(300, ErrorMessage = "validation.fullNameMaxLength")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.phoneNumberRequired")]
    [MaxLength(30, ErrorMessage = "validation.phoneNumberMaxLength")]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "validation.emailMaxLength")]
    public string? Email { get; set; }

    [MaxLength(200, ErrorMessage = "validation.specializationMaxLength")]
    public string? Specialization { get; set; }

    [MaxLength(200, ErrorMessage = "validation.hospitalMaxLength")]
    public string? Hospital { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            yield return new ValidationResult(ValidationKeys.FullNameFirstAndLast, new[] { nameof(FullName) });

        if (!string.IsNullOrWhiteSpace(Phone) && !EgyptianPhoneNumber.IsValid(Phone))
            yield return new ValidationResult(ValidationKeys.PhoneEgyptian, new[] { nameof(Phone) });
    }
}
