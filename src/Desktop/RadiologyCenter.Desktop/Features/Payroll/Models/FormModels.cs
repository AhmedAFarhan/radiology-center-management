using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Payroll.Models;

internal sealed class SalaryFormModel
{
    public decimal BaseSalary { get; set; }

    [Required(ErrorMessage = "validation.salaryTypeRequired")]
    public string SalaryType { get; set; } = "Monthly";

    [Required(ErrorMessage = "validation.effectiveDateRequired")]
    public DateTime? EffectiveDate { get; set; }
}

internal sealed class SalaryComponentFormModel
{
    [Required(ErrorMessage = "validation.nameRequired")]
    [MaxLength(100, ErrorMessage = "validation.nameMaxLength100")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.kindRequired")]
    public string Kind { get; set; } = "Earning";

    public string? Frequency { get; set; }

    public bool IsPercentage { get; set; }

    public bool IsPerWorkDay { get; set; }

    public decimal DefaultValue { get; set; }
}

internal sealed class AllowanceFormModel
{
    [Required(ErrorMessage = "validation.allowanceNameRequired")]
    [MaxLength(100, ErrorMessage = "validation.allowanceNameMaxLength")]
    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? Frequency { get; set; }

    public bool IsPerWorkDay { get; set; }

    [Required(ErrorMessage = "validation.effectiveDateRequired")]
    public DateTime? EffectiveDate { get; set; }

    public DateTime? EndDate { get; set; }
}

internal sealed class ExaminationFeeFormModel
{
    [Required(ErrorMessage = "validation.roleRequired")]
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
    [Required(ErrorMessage = "validation.employeeRequired")]
    public StaffDto? SelectedStaff { get; set; }
}

internal sealed class PayRunFormModel
{
    public DateTime? RunFrom { get; set; }

    public DateTime? RunTo { get; set; }

    public string? Notes { get; set; }
}
