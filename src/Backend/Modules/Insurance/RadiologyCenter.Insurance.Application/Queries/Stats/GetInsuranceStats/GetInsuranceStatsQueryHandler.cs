using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Application.Queries.Stats.GetInsuranceStats;

public static class GetInsuranceStatsQueryHandler
{
    public static async Task<Result<InsuranceStatsDto>> HandleAsync(
        GetInsuranceStatsQuery query,
        IInsuranceCompanyRepository companyRepository,
        IInsurancePolicyRepository policyRepository,
        IPreAuthorizationRepository preAuthorizationRepository,
        IClaimRepository claimRepository,
        CancellationToken ct)
    {
        var companies = await companyRepository.GetAllAsync(ct);
        var policies = await policyRepository.GetAllAsync(ct);
        var preAuthorizations = await preAuthorizationRepository.GetAllAsync(ct);
        var claims = await claimRepository.GetAllAsync(ct);

        var outstandingAmount = claims
            .Where(c => c.Status == ClaimStatus.Approved || c.Status == ClaimStatus.Paid)
            .Sum(c => c.RemainingOwed);

        var stats = new InsuranceStatsDto(
            companies.Count,
            policies.Count,
            policies.Count(p => p.IsActive),
            preAuthorizations.Count(p => p.Status == PreAuthorizationStatus.Requested),
            preAuthorizations.Count(p => p.Status == PreAuthorizationStatus.Approved),
            claims.Count(c => c.Status == ClaimStatus.Draft),
            claims.Count(c => c.Status == ClaimStatus.Submitted),
            claims.Count(c => c.Status == ClaimStatus.Approved),
            outstandingAmount);

        return Result.Success(stats);
    }
}