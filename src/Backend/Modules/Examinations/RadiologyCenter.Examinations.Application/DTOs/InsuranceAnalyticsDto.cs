namespace RadiologyCenter.Examinations.Application.DTOs;

public sealed record InsuranceAnalyticsDto(
    int TotalClaims,
    int DraftClaims,
    int SubmittedClaims,
    int ApprovedClaims,
    int RejectedClaims,
    int PaidClaims,
    decimal TotalBilledAmount,
    decimal TotalPayerShare,
    decimal TotalPatientShare,
    decimal TotalSettled,
    decimal OutstandingAmount,
    decimal ApprovalRate,
    IReadOnlyList<InsuranceClaimRowDto> ClaimRows);

public sealed record InsuranceClaimRowDto(
    Guid ClaimId,
    string PatientName,
    string InsuranceCompany,
    string PolicyNumber,
    decimal BilledAmount,
    decimal PayerShare,
    decimal PatientShare,
    string Status,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    decimal SettledAmount,
    decimal RemainingOwed);
