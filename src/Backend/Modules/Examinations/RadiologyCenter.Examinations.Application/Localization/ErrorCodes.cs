namespace RadiologyCenter.Examinations.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string ScheduledTimePast = "Examination.ScheduledTimePast";
    public const string PercentageDiscountMax = "Examination.PercentageDiscountMax";
    public const string DuplicateItem = "Examination.DuplicateItem";
    public const string ExaminationNotFound = "Examination.ExaminationNotFound";
    public const string ExaminationTypeNotFound = "Examination.ExaminationTypeNotFound";
    public const string ExaminationTypeItemNotFound = "Examination.ExaminationTypeItemNotFound";
    public const string ItemAlreadyInPreferences = "Examination.ItemAlreadyInPreferences";
    public const string PaymentForCancelledExamination = "Examination.PaymentForCancelledExamination";
    public const string PaymentExceedsRemaining = "Examination.PaymentExceedsRemaining";
    public const string AuthenticationRequired = "Examination.AuthenticationRequired";
    public const string PaidAmountImmutable = "Examination.PaidAmountImmutable";
    public const string StaffNotAssigned = "Examination.StaffNotAssigned";
}
