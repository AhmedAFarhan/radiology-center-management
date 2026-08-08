namespace RadiologyCenter.Insurance.Application.DTOs;

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