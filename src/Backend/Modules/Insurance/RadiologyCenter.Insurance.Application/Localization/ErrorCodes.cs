namespace RadiologyCenter.Insurance.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string ClaimNotFound = "Insurance.ClaimNotFound";
    public const string PolicyNotFound = "Insurance.PolicyNotFound";
    public const string PreAuthorizationNotFound = "Insurance.PreAuthorizationNotFound";
    public const string CompanyNotFound = "Insurance.CompanyNotFound";
    public const string PolicyDocumentNotFound = "Insurance.PolicyDocumentNotFound";
    public const string PreAuthorizationDocumentNotFound = "Insurance.PreAuthorizationDocumentNotFound";
    public const string ApprovedAmountRequired = "Insurance.ApprovedAmountRequired";
    public const string RejectionCodeRequired = "Insurance.RejectionCodeRequired";
    public const string RejectionReasonRequired = "Insurance.RejectionReasonRequired";
    public const string DenialReasonRequired = "Insurance.DenialReasonRequired";
    public const string UnsupportedDecision = "Insurance.UnsupportedDecision";
    public const string UnsupportedPolicyAction = "Insurance.UnsupportedPolicyAction";
    public const string PolicyNotActive = "Insurance.PolicyNotActive";
    public const string PolicyPatientMismatch = "Insurance.PolicyPatientMismatch";
    public const string ClaimAlreadyExists = "Insurance.ClaimAlreadyExists";
    public const string PreAuthorizationAlreadyExists = "Insurance.PreAuthorizationAlreadyExists";
    public const string PreAuthorizationExaminationMismatch = "Insurance.PreAuthorizationExaminationMismatch";
    public const string PreAuthorizationPatientMismatch = "Insurance.PreAuthorizationPatientMismatch";
    public const string PreAuthorizationPolicyMismatch = "Insurance.PreAuthorizationPolicyMismatch";
    public const string PreAuthorizationNotApproved = "Insurance.PreAuthorizationNotApproved";
    public const string PreAuthorizationNoApprovedAmount = "Insurance.PreAuthorizationNoApprovedAmount";
    public const string BilledAmountExceedsApproved = "Insurance.BilledAmountExceedsApproved";
    public const string SettlementExceedsRemaining = "Insurance.SettlementExceedsRemaining";
    public const string CompanyNameExists = "Insurance.CompanyNameExists";
    public const string PolicyNumberExists = "Insurance.PolicyNumberExists";
    public const string InvalidDocumentType = "Insurance.InvalidDocumentType";
}
