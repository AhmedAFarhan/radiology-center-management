namespace RadiologyCenter.Desktop.Models;

public sealed record InsuranceCompanyDto(
    string Id,
    string Name,
    string? TaxId,
    string? Address,
    string? Phone,
    string? Email);

public sealed class InsuranceCompanyInput
{
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public sealed record InsurancePolicyDto(
    string Id,
    string CompanyId,
    string PatientId,
    string PolicyNumber,
    decimal CoveragePercent,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    string Status,
    bool IsGovernment);

public sealed record InsurancePolicyListItemDto(
    string Id,
    string CompanyId,
    string CompanyName,
    string PatientId,
    string PatientName,
    string PolicyNumber,
    decimal CoveragePercent,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    string Status,
    bool IsGovernment,
    bool IsActive);

public sealed class InsurancePolicyInput
{
    public string CompanyId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal CoveragePercent { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsGovernment { get; set; }
}

public sealed class UpdatePolicyCoverageInput
{
    public decimal CoveragePercent { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class ChangePolicyStatusInput
{
    public string Action { get; set; } = string.Empty;
}

public sealed record PreAuthorizationDto(
    string Id,
    string ExaminationId,
    string PatientId,
    string PolicyId,
    decimal EstimatedAmount,
    string Status,
    DateTime RequestedAt,
    DateTime? DecidedAt,
    decimal? ApprovedAmount,
    string? RejectionReason,
    bool IsGovernment,
    IReadOnlyList<PreAuthorizationDocumentDto>? Documents = null);

public sealed record PreAuthorizationListItemDto(
    string Id,
    string ExaminationId,
    string ExaminationTypeName,
    string PatientId,
    string PatientName,
    string PolicyId,
    string PolicyNumber,
    decimal EstimatedAmount,
    string Status,
    DateTime RequestedAt,
    DateTime? DecidedAt,
    decimal? ApprovedAmount,
    string? RejectionReason,
    bool IsGovernment);

public sealed class CreatePreAuthorizationInput
{
    public string ExaminationId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string PolicyId { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
}

public sealed class DecidePreAuthorizationInput
{
    public string Decision { get; set; } = string.Empty;
    public decimal? ApprovedAmount { get; set; }
    public string? RejectionReason { get; set; }
}

public sealed record ClaimDto(
    string Id,
    string ExaminationId,
    string PatientId,
    string PolicyId,
    string PreAuthorizationId,
    decimal BilledAmount,
    decimal PayerShare,
    decimal PatientShare,
    string Status,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    DateTime? PaidAt,
    decimal TotalSettled,
    decimal RemainingOwed,
    IReadOnlyList<SettlementDto> Settlements,
    IReadOnlyList<ClaimRejectionDto> Rejections);

public sealed record ClaimListItemDto(
    string Id,
    string ExaminationId,
    string ExaminationTypeName,
    string PatientId,
    string PatientName,
    string PolicyId,
    string PolicyNumber,
    string PreAuthorizationId,
    decimal BilledAmount,
    decimal PayerShare,
    decimal PatientShare,
    string Status,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    DateTime? PaidAt,
    decimal TotalSettled,
    decimal RemainingOwed);

public sealed class CreateClaimInput
{
    public string ExaminationId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string PolicyId { get; set; } = string.Empty;
    public string PreAuthorizationId { get; set; } = string.Empty;
    public decimal BilledAmount { get; set; }
}

public sealed class AdjudicateClaimInput
{
    public string Decision { get; set; } = string.Empty;
    public decimal? ApprovedAmount { get; set; }
    public string? RejectionCode { get; set; }
    public string? RejectionReason { get; set; }
}

public sealed class RecordSettlementInput
{
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}

public sealed record SettlementDto(
    string Id,
    decimal Amount,
    string Method,
    DateTime SettledAt,
    string? Reference);

public sealed record ClaimRejectionDto(
    string Id,
    string Code,
    string Reason,
    DateTime RejectedAt);

public sealed record PolicyDocumentDto(
    string Id,
    string PolicyId,
    string Type,
    string FileName,
    string ContentType,
    long SizeInBytes,
    DateTime UploadedAt);

public sealed record PreAuthorizationDocumentDto(
    string Id,
    string PreAuthorizationId,
    string Type,
    string FileName,
    string ContentType,
    long SizeInBytes,
    DateTime UploadedAt);

public sealed record InsuranceStatsDto(
    int TotalCompanies,
    int TotalPolicies,
    int ActivePolicies,
    int PendingPreAuthorizations,
    int ApprovedPreAuthorizations,
    int DraftClaims,
    int SubmittedClaims,
    int ApprovedClaims,
    decimal OutstandingAmount);
