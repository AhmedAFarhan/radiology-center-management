using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaims;

public static class GetClaimsQueryHandler
{
    public static async Task<Result<PagedResult<ClaimListItemDto>>> HandleAsync(
        GetClaimsQuery query,
        IClaimRepository claimRepository,
        IInsurancePolicyRepository policyRepository,
        IInsuranceDirectory directory,
        CancellationToken ct)
    {
        var paged = await claimRepository.GetPagedAsync(query.Request, ct);

        var patientIds = paged.Items.Select(c => c.PatientId).Distinct().ToList();
        var examinationIds = paged.Items.Select(c => c.ExaminationId).Distinct().ToList();
        var policyIds = paged.Items.Select(c => c.PolicyId).Distinct().ToList();

        var patientNames = await directory.ResolvePatientNamesAsync(patientIds, ct);
        var examinationTypeNames = await directory.ResolveExaminationTypeNamesAsync(examinationIds, ct);
        var policies = await policyRepository.GetByIdsAsync(policyIds, ct);
        var policyNumbers = policies.ToDictionary(p => p.Id, p => p.PolicyNumber);

        var items = paged.Items.Select(c => new ClaimListItemDto(
            c.Id,
            c.ExaminationId,
            examinationTypeNames.GetValueOrDefault(c.ExaminationId) ?? string.Empty,
            c.PatientId,
            patientNames.GetValueOrDefault(c.PatientId) ?? string.Empty,
            c.PolicyId,
            policyNumbers.GetValueOrDefault(c.PolicyId) ?? string.Empty,
            c.PreAuthorizationId,
            c.BilledAmount,
            c.PayerShare,
            c.PatientShare,
            c.Status.LocalizedName(),
            c.SubmittedAt,
            c.ApprovedAt,
            c.PaidAt,
            c.TotalSettled,
            c.RemainingOwed)).ToList();

        return Result.Success(new PagedResult<ClaimListItemDto>(
            items,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}