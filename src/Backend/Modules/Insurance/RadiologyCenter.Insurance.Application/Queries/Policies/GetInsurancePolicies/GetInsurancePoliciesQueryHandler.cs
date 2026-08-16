using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetInsurancePolicies;

public static class GetInsurancePoliciesQueryHandler
{
    public static async Task<Result<PagedResult<InsurancePolicyListItemDto>>> HandleAsync(
        GetInsurancePoliciesQuery query,
        IInsurancePolicyRepository policyRepository,
        IInsuranceCompanyRepository companyRepository,
        IInsuranceDirectory directory,
        CancellationToken ct)
    {
        var paged = await policyRepository.GetPagedAsync(query.Request, ct);

        var patientIds = paged.Items.Select(p => p.PatientId).Distinct().ToList();
        var companyIds = paged.Items.Select(p => p.CompanyId).Distinct().ToList();

        var patientNames = await directory.ResolvePatientNamesAsync(patientIds, ct);
        var companyNames = await companyRepository.GetNamesByIdsAsync(companyIds, ct);

        var items = paged.Items.Select(p => new InsurancePolicyListItemDto(
            p.Id,
            p.CompanyId,
            companyNames.GetValueOrDefault(p.CompanyId) ?? string.Empty,
            p.PatientId,
            patientNames.GetValueOrDefault(p.PatientId) ?? string.Empty,
            p.PolicyNumber,
            p.CoveragePercent,
            p.EffectiveFrom,
            p.EffectiveTo,
            p.Status.LocalizedName(),
            p.IsGovernment,
            p.IsActive)).ToList();

        return Result.Success(new PagedResult<InsurancePolicyListItemDto>(
            items,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}