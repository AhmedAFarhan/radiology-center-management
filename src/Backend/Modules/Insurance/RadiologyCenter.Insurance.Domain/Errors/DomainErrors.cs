namespace RadiologyCenter.Insurance.Domain.Errors;

/// <summary>
/// Stable semantic codes for domain-invariant violations. Thrown as
/// <see cref="DomainException"/> codes and resolved through the "codes"
/// section of the module JSON resource files.
/// </summary>
public static class DomainErrors
{
    public const string BilledAmountNegative = "Insurance.BilledAmountNegative";
    public const string PayerShareNegative = "Insurance.PayerShareNegative";
    public const string PatientShareNegative = "Insurance.PatientShareNegative";
    public const string SharesExceedBilled = "Insurance.SharesExceedBilled";
    public const string ClaimSubmitInvalidStatus = "Insurance.ClaimSubmitInvalidStatus";
    public const string ClaimNotSubmittedAdjudication = "Insurance.ClaimNotSubmittedAdjudication";
    public const string ApprovedAmountNegative = "Insurance.ApprovedAmountNegative";
    public const string ApprovedAmountExceedsPayerShare = "Insurance.ApprovedAmountExceedsPayerShare";
    public const string ClaimNotRejectedResubmit = "Insurance.ClaimNotRejectedResubmit";
    public const string ClaimNotApprovedSettlement = "Insurance.ClaimNotApprovedSettlement";
    public const string SettlementAmountPositive = "Insurance.SettlementAmountPositive";
    public const string SettlementExceedsOwed = "Insurance.SettlementExceedsOwed";
    public const string CoveragePercentRange = "Insurance.CoveragePercentRange";
    public const string UpdateExpiredPolicy = "Insurance.UpdateExpiredPolicy";
    public const string ReactivateExpiredPolicy = "Insurance.ReactivateExpiredPolicy";
    public const string DocumentSizePositive = "Insurance.DocumentSizePositive";
    public const string EstimatedAmountNegative = "Insurance.EstimatedAmountNegative";
    public const string DocumentsRequestedOnly = "Insurance.DocumentsRequestedOnly";
    public const string GovernmentDocRequired = "Insurance.GovernmentDocRequired";
    public const string PreAuthorizationAlreadyDecided = "Insurance.PreAuthorizationAlreadyDecided";
    public const string EffectiveToBeforeEffectiveFrom = "Insurance.EffectiveToBeforeEffectiveFrom";
}
