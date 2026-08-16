namespace RadiologyCenter.Payroll.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
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
}
