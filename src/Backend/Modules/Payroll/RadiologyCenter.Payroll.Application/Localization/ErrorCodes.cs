namespace RadiologyCenter.Payroll.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    // Domain errors
    public const string PayRunNotFound = "Payroll.PayRunNotFound";
    public const string StaffNotFound = "Payroll.StaffNotFound";
    public const string SalaryComponentNotFound = "Payroll.SalaryComponentNotFound";
    public const string ExaminationTypeNotFound = "Payroll.ExaminationTypeNotFound";
    public const string ReferralDoctorNotFound = "Payroll.ReferralDoctorNotFound";
    public const string PayslipCalculationFailed = "Payroll.PayslipCalculationFailed";
    public const string PayRunCannotRecompute = "Payroll.PayRunCannotRecompute";
    public const string PayRunCannotDelete = "Payroll.PayRunCannotDelete";
    public const string PayRunOverlapExists = "Payroll.PayRunOverlapExists";
    public const string PayslipComponentNotFound = "Payroll.PayslipComponentNotFound";
    public const string PayRunEndOnOrAfterStart = "Payroll.PayRunEndOnOrAfterStart";
    public const string EndDateOnOrAfterEffectiveDate = "Payroll.EndDateOnOrAfterEffectiveDate";
    public const string PercentageAmountMax = "Payroll.PercentageAmountMax";

    // Validation error codes
    public const string PayRunIdRequired = "Payroll.PayRunIdRequired";
    public const string StaffIdRequired = "Payroll.StaffIdRequired";
    public const string SalaryIdRequired = "Payroll.SalaryIdRequired";
    public const string SalaryComponentIdRequired = "Payroll.SalaryComponentIdRequired";
    public const string AllowanceAssignmentIdRequired = "Payroll.AllowanceAssignmentIdRequired";
    public const string ExaminationFeeIdRequired = "Payroll.ExaminationFeeIdRequired";
    public const string ReferralFeeIdRequired = "Payroll.ReferralFeeIdRequired";
    public const string IdRequired = "Payroll.IdRequired";
    public const string NameRequired = "Payroll.NameRequired";
    public const string NameTooLong = "Payroll.NameTooLong";
    public const string AmountCannotBeNegative = "Payroll.AmountCannotBeNegative";
    public const string EffectiveDateRequired = "Payroll.EffectiveDateRequired";
    public const string SalaryTypeRequired = "Payroll.SalaryTypeRequired";
    public const string KindRequired = "Payroll.KindRequired";
    public const string DefaultValueCannotBeNegative = "Payroll.DefaultValueCannotBeNegative";
    public const string RunFromRequired = "Payroll.RunFromRequired";
    public const string RunToRequired = "Payroll.RunToRequired";
    public const string ExaminationTypeIdRequired = "Payroll.ExaminationTypeIdRequired";
    public const string ReferralDoctorIdRequired = "Payroll.ReferralDoctorIdRequired";
    public const string RoleRequired = "Payroll.RoleRequired";
    public const string RequestRequired = "Payroll.RequestRequired";
    public const string PageNumberMustBePositive = "Payroll.PageNumberMustBePositive";
    public const string PageSizeMustBeBetween = "Payroll.PageSizeMustBeBetween";
}
