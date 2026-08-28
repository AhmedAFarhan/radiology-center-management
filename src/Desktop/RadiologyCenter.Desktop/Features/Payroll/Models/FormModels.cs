using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Payroll.Models;

internal sealed class SalaryFormModel
{
    public decimal BaseSalary { get; set; }

    [Required(ErrorMessage = "Salary type is required.")]
    public string SalaryType { get; set; } = "Monthly";

    [Required(ErrorMessage = "Effective date is required.")]
    public DateTime? EffectiveDate { get; set; }
}

internal sealed class SalaryComponentFormModel
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kind is required.")]
    public string Kind { get; set; } = "Earning";

    public string? Frequency { get; set; }

    public bool IsPercentage { get; set; }

    public bool IsPerWorkDay { get; set; }

    public decimal DefaultValue { get; set; }
}

internal sealed class AllowanceFormModel
{
    [Required(ErrorMessage = "Allowance name is required.")]
    [MaxLength(100, ErrorMessage = "Allowance name must be 100 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? Frequency { get; set; }

    public bool IsPerWorkDay { get; set; }

    [Required(ErrorMessage = "Effective date is required.")]
    public DateTime? EffectiveDate { get; set; }

    public DateTime? EndDate { get; set; }
}

internal sealed class ExaminationFeeFormModel
{
    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; } = "Radiologist";

    public decimal Amount { get; set; }

    public bool IsPercentage { get; set; }
}

internal sealed class ReferralFeeFormModel
{
    public decimal Amount { get; set; }

    public bool IsPercentage { get; set; }
}

internal sealed class AddPayslipFormModel
{
    [Required(ErrorMessage = "Employee is required.")]
    public StaffDto? SelectedStaff { get; set; }
}

internal sealed class PayRunFormModel
{
    public DateTime? RunFrom { get; set; }

    public DateTime? RunTo { get; set; }

    public string? Notes { get; set; }
}
