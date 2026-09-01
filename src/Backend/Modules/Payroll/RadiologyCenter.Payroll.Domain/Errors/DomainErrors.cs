namespace RadiologyCenter.Payroll.Domain.Errors;

/// <summary>
/// Stable semantic codes for domain-invariant violations. Thrown as
/// <see cref="DomainException"/> codes and resolved through the "codes"
/// section of the module JSON resource files.
/// </summary>
public static class DomainErrors
{
    public const string RunToBeforeRunFrom = "Payroll.RunToBeforeRunFrom";
    public const string GrossSalaryNegative = "Payroll.GrossSalaryNegative";
    public const string UnpaidLeaveDaysNegative = "Payroll.UnpaidLeaveDaysNegative";
    public const string UnpaidLeaveDeductionNegative = "Payroll.UnpaidLeaveDeductionNegative";
    public const string DuplicatePayslip = "Payroll.DuplicatePayslip";
    public const string PayslipNotFound = "Payroll.PayslipNotFound";
    public const string PayslipComponentNotFound = "Payroll.PayslipComponentNotFound";
    public const string PayRunNotEditable = "Payroll.PayRunNotEditable";
    public const string InvalidPayRunTransition = "Payroll.InvalidPayRunTransition";
    public const string AllowanceAmountNegative = "Payroll.AllowanceAmountNegative";
    public const string EndDateBeforeEffectiveDate = "Payroll.EndDateBeforeEffectiveDate";
    public const string ExaminationFeeNegative = "Payroll.ExaminationFeeNegative";
    public const string PercentageFeeMax = "Payroll.PercentageFeeMax";
    public const string ReferralFeeNegative = "Payroll.ReferralFeeNegative";
    public const string BaseSalaryNegative = "Payroll.BaseSalaryNegative";
    public const string DefaultValueNegative = "Payroll.DefaultValueNegative";
    public const string DuplicateReferralFeeStatement = "Payroll.DuplicateReferralFeeStatement";
}
