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
    public const string InvalidStatusTransition = "Examination.InvalidStatusTransition";
    public const string StaffNotAssigned = "Examination.StaffNotAssigned";
    public const string EquipmentNotAssigned = "Examination.EquipmentNotAssigned";
    public const string EquipmentOverlap = "Examination.EquipmentOverlap";
    public const string RadiologistOverlap = "Examination.RadiologistOverlap";

    public const string ExaminationIdRequired = "Examination.ExaminationIdRequired";
    public const string ItemIdRequired = "Examination.ItemIdRequired";
    public const string QuantityMustBePositive = "Examination.QuantityMustBePositive";
    public const string NotesTooLong = "Examination.NotesTooLong";
    public const string RadiologistIdRequired = "Examination.RadiologistIdRequired";
    public const string TechnicianIdRequired = "Examination.TechnicianIdRequired";
    public const string EquipmentIdRequired = "Examination.EquipmentIdRequired";
    public const string PatientIdRequired = "Examination.PatientIdRequired";
    public const string ExaminationTypeIdRequired = "Examination.ExaminationTypeIdRequired";
    public const string ScheduledAtRequired = "Examination.ScheduledAtRequired";
    public const string PriorityRequired = "Examination.PriorityRequired";
    public const string ClinicalIndicationRequired = "Examination.ClinicalIndicationRequired";
    public const string ClinicalIndicationTooLong = "Examination.ClinicalIndicationTooLong";
    public const string DiscountCannotBeNegative = "Examination.DiscountCannotBeNegative";
    public const string PaidCannotBeNegative = "Examination.PaidCannotBeNegative";
    public const string DescriptionTooLong = "Examination.DescriptionTooLong";
    public const string StudyInstanceUidTooLong = "Examination.StudyInstanceUidTooLong";
    public const string AccessionNumberTooLong = "Examination.AccessionNumberTooLong";
    public const string ExaminationItemIdRequired = "Examination.ExaminationItemIdRequired";
    public const string ExaminationTypeItemIdRequired = "Examination.ExaminationTypeItemIdRequired";
    public const string RequestRequired = "Examination.RequestRequired";
    public const string PageNumberMustBePositive = "Examination.PageNumberMustBePositive";
    public const string PageSizeMustBeBetween = "Examination.PageSizeMustBeBetween";
}
