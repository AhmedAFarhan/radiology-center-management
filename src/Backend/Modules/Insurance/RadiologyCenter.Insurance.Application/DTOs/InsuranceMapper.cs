using RadiologyCenter.Insurance.Domain.Entities;

namespace RadiologyCenter.Insurance.Application.DTOs;

internal static class InsuranceMapper
{
    public static InsuranceCompanyDto ToDto(this InsuranceCompany company) =>
        new(
            company.Id,
            company.Name,
            company.TaxId,
            company.Address,
            company.Phone,
            company.Email);

    public static InsurancePolicyDto ToDto(this InsurancePolicy policy) =>
        new(
            policy.Id,
            policy.CompanyId,
            policy.PatientId,
            policy.PolicyNumber,
            policy.CoveragePercent,
            policy.EffectiveFrom,
            policy.EffectiveTo,
            policy.Status.Name);

    public static PreAuthorizationDto ToDto(this PreAuthorization preAuthorization) =>
        new(
            preAuthorization.Id,
            preAuthorization.ExaminationId,
            preAuthorization.PatientId,
            preAuthorization.PolicyId,
            preAuthorization.EstimatedAmount,
            preAuthorization.Status.Name,
            preAuthorization.RequestedAt,
            preAuthorization.DecidedAt,
            preAuthorization.ApprovedAmount,
            preAuthorization.RejectionReason);

    public static ClaimDto ToDto(this Claim claim) =>
        new(
            claim.Id,
            claim.ExaminationId,
            claim.PatientId,
            claim.PolicyId,
            claim.PreAuthorizationId,
            claim.BilledAmount,
            claim.PayerShare,
            claim.PatientShare,
            claim.Status.Name,
            claim.SubmittedAt,
            claim.ApprovedAt,
            claim.PaidAt,
            claim.TotalSettled,
            claim.RemainingOwed,
            claim.Settlements.Select(s => s.ToDto()).ToList(),
            claim.Rejections.Select(r => r.ToDto()).ToList());

    public static SettlementDto ToDto(this Settlement settlement) =>
        new(
            settlement.Id,
            settlement.Amount,
            settlement.Method.Name,
            settlement.SettledAt,
            settlement.Reference);

    public static ClaimRejectionDto ToDto(this ClaimRejection rejection) =>
        new(
            rejection.Id,
            rejection.Code.Name,
            rejection.Reason,
            rejection.RejectedAt);
}