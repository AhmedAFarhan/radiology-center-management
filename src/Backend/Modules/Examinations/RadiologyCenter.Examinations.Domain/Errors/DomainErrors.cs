namespace RadiologyCenter.Examinations.Domain.Errors;

/// <summary>
/// Stable semantic codes for domain-invariant violations. Thrown as
/// <see cref="DomainException"/> codes and resolved through the "codes"
/// section of the module JSON resource files.
/// </summary>
public static class DomainErrors
{
    public const string PriceNegative = "Examination.PriceNegative";
    public const string DiscountNegative = "Examination.DiscountNegative";
    public const string PercentageDiscountMax = "Examination.PercentageDiscountMax";
    public const string PaidAmountNegative = "Examination.PaidAmountNegative";
    public const string PaidAmountImmutable = "Examination.PaidAmountImmutable";
    public const string ScheduledTimeDefault = "Examination.ScheduledTimeDefault";
    public const string ScheduledTimePast = "Examination.ScheduledTimePast";
    public const string PaymentNegative = "Examination.PaymentNegative";
    public const string PaymentExceedsRemaining = "Examination.PaymentExceedsRemaining";
    public const string RefundNegative = "Examination.RefundNegative";
    public const string RefundExceedsPaid = "Examination.RefundExceedsPaid";
    public const string DuplicateItem = "Examination.DuplicateItem";
    public const string RequiredItemCannotRemove = "Examination.RequiredItemCannotRemove";
    public const string ItemNotOnExamination = "Examination.ItemNotOnExamination";
    public const string ItemsCannotBeModified = "Examination.ItemsCannotBeModified";
    public const string InvalidStatusTransition = "Examination.InvalidStatusTransition";
    public const string StaffNotAssigned = "Examination.StaffNotAssigned";
    public const string RadiologistFeeNegative = "Examination.RadiologistFeeNegative";
    public const string TechnicianFeeNegative = "Examination.TechnicianFeeNegative";
    public const string ReferralFeeNegative = "Examination.ReferralFeeNegative";
    public const string RemainingAmountNegative = "Examination.RemainingAmountNegative";
    public const string UnitCostNegative = "Examination.UnitCostNegative";
}
