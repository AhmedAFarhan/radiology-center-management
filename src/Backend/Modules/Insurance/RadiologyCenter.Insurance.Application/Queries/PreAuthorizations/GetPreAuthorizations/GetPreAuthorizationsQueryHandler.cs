using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizations;

public static class GetPreAuthorizationsQueryHandler
{
    public static async Task<Result<PagedResult<PreAuthorizationListItemDto>>> HandleAsync(
        GetPreAuthorizationsQuery query,
        IPreAuthorizationRepository preAuthorizationRepository,
        IInsurancePolicyRepository policyRepository,
        IInsuranceDirectory directory,
        CancellationToken ct)
    {
        var paged = await preAuthorizationRepository.GetPagedAsync(query.Request, ct);

        var patientIds = paged.Items.Select(p => p.PatientId).Distinct().ToList();
        var examinationIds = paged.Items.Select(p => p.ExaminationId).Distinct().ToList();
        var policyIds = paged.Items.Select(p => p.PolicyId).Distinct().ToList();

        var patientNames = await directory.ResolvePatientNamesAsync(patientIds, ct);
        var examinationTypeNames = await directory.ResolveExaminationTypeNamesAsync(examinationIds, ct);
        var policies = await policyRepository.GetByIdsAsync(policyIds, ct);
        var policyNumbers = policies.ToDictionary(p => p.Id, p => p.PolicyNumber);

        var items = paged.Items.Select(p => new PreAuthorizationListItemDto(
            p.Id,
            p.ExaminationId,
            examinationTypeNames.GetValueOrDefault(p.ExaminationId) ?? string.Empty,
            p.PatientId,
            patientNames.GetValueOrDefault(p.PatientId) ?? string.Empty,
            p.PolicyId,
            policyNumbers.GetValueOrDefault(p.PolicyId) ?? string.Empty,
            p.EstimatedAmount,
            p.Status.LocalizedName(),
            p.RequestedAt,
            p.DecidedAt,
            p.ApprovedAmount,
            p.RejectionReason,
            p.IsGovernment)).ToList();

        return Result.Success(new PagedResult<PreAuthorizationListItemDto>(
            items,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}