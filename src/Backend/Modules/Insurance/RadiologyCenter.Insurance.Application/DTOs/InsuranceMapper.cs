using RadiologyCenter.BuildingBlocks.Application.Localization;
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
            policy.Status.LocalizedName(),
            policy.IsGovernment);

    public static PreAuthorizationDto ToDto(this PreAuthorization preAuthorization) =>
        new(
            preAuthorization.Id,
            preAuthorization.ExaminationId,
            preAuthorization.PatientId,
            preAuthorization.PolicyId,
            preAuthorization.EstimatedAmount,
            preAuthorization.Status.LocalizedName(),
            preAuthorization.RequestedAt,
            preAuthorization.DecidedAt,
            preAuthorization.ApprovedAmount,
            preAuthorization.RejectionReason,
            preAuthorization.IsGovernment,
            preAuthorization.Documents.Select(d => d.ToDto()).ToList(),
            preAuthorization.Status.Name);

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
            claim.Status.LocalizedName(),
            claim.SubmittedAt,
            claim.ApprovedAt,
            claim.PaidAt,
            claim.TotalSettled,
            claim.RemainingOwed,
            claim.Settlements.Select(s => s.ToDto()).ToList(),
            claim.Rejections.Select(r => r.ToDto()).ToList(),
            claim.Status.Name);

    public static SettlementDto ToDto(this Settlement settlement) =>
        new(
            settlement.Id,
            settlement.Amount,
            settlement.Method.LocalizedName(),
            settlement.SettledAt,
            settlement.Reference);

    public static ClaimRejectionDto ToDto(this ClaimRejection rejection) =>
        new(
            rejection.Id,
            rejection.Code.LocalizedName(),
            rejection.Reason,
            rejection.RejectedAt);

    public static PolicyDocumentDto ToDto(this PolicyDocument document) =>
        new(
            document.Id,
            document.PolicyId,
            document.Type.LocalizedName(),
            document.FileName,
            document.ContentType,
            document.SizeInBytes,
            document.UploadedAt);

    public static PreAuthorizationDocumentDto ToDto(this PreAuthorizationDocument document) =>
        new(
            document.Id,
            document.PreAuthorizationId,
            document.Type.LocalizedName(),
            document.FileName,
            document.ContentType,
            document.SizeInBytes,
            document.UploadedAt);
}