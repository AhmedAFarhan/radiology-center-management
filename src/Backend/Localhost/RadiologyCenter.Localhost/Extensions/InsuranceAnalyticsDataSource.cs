using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;
using RadiologyCenter.Insurance.Application.Abstractions;

namespace RadiologyCenter.Localhost.Extensions;

public sealed class InsuranceAnalyticsDataSource : IInsuranceAnalyticsDataSource
{
    private readonly IClaimRepository _claimRepository;
    private readonly IInsurancePolicyRepository _policyRepository;
    private readonly IInsuranceCompanyRepository _companyRepository;
    private readonly ISettlementRepository _settlementRepository;
    private readonly IInsuranceDirectory _directory;

    public InsuranceAnalyticsDataSource(
        IClaimRepository claimRepository,
        IInsurancePolicyRepository policyRepository,
        IInsuranceCompanyRepository companyRepository,
        ISettlementRepository settlementRepository,
        IInsuranceDirectory directory)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
        _companyRepository = companyRepository;
        _settlementRepository = settlementRepository;
        _directory = directory;
    }

    public async Task<IReadOnlyList<InsuranceClaimRowDto>> GetClaimRowsAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _claimRepository.GetByDateRangeAsync(from, to, ct);
        if (claims.Count == 0)
            return [];

        var policyIds = claims.Select(c => c.PolicyId).Distinct().ToList();
        var policies = await _policyRepository.GetByIdsAsync(policyIds, ct);
        var policyLookup = policies.ToDictionary(p => p.Id);

        var companyIds = policies.Select(p => p.CompanyId).Distinct().ToList();
        var companyNames = await _companyRepository.GetNamesByIdsAsync(companyIds, ct);

        var patientIds = claims.Select(c => c.PatientId).Distinct().ToList();
        var patientNames = await _directory.ResolvePatientNamesAsync(patientIds, ct);

        var claimIds = claims.Select(c => c.Id).ToList();
        var settlements = await _settlementRepository.GetByClaimIdsAsync(claimIds, ct);
        var settlementLookup = settlements.GroupBy(s => s.ClaimId).ToDictionary(g => g.Key, g => g.Sum(s => s.Amount));

        return claims.Select(c =>
        {
            var policy = policyLookup.GetValueOrDefault(c.PolicyId);
            var companyName = policy is not null ? companyNames.GetValueOrDefault(policy.CompanyId, "") : "";
            var settled = settlementLookup.GetValueOrDefault(c.Id, 0m);
            var name = patientNames.TryGetValue(c.PatientId, out var n) ? n : "";

            return new InsuranceClaimRowDto(
                c.Id,
                name,
                companyName,
                policy?.PolicyNumber ?? "",
                c.BilledAmount,
                c.PayerShare,
                c.PatientShare,
                c.Status.Name,
                c.SubmittedAt,
                c.ApprovedAt,
                settled,
                c.PayerShare - settled);
        }).ToList();
    }

    public async Task<int> GetTotalClaimsAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _claimRepository.GetByDateRangeAsync(from, to, ct);
        return claims.Count;
    }

    public async Task<int> GetClaimsByStatusAsync(string status, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _claimRepository.GetByDateRangeAsync(from, to, ct);
        return claims.Count(c => c.Status.Name == status);
    }

    public async Task<decimal> GetTotalBilledAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _claimRepository.GetByDateRangeAsync(from, to, ct);
        return claims.Sum(c => c.BilledAmount);
    }

    public async Task<decimal> GetTotalPayerShareAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _claimRepository.GetByDateRangeAsync(from, to, ct);
        return claims.Sum(c => c.PayerShare);
    }

    public async Task<decimal> GetTotalPatientShareAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _claimRepository.GetByDateRangeAsync(from, to, ct);
        return claims.Sum(c => c.PatientShare);
    }

    public async Task<decimal> GetTotalSettledAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _claimRepository.GetByDateRangeAsync(from, to, ct);
        var claimIds = claims.Select(c => c.Id).ToList();
        var settlements = await _settlementRepository.GetByClaimIdsAsync(claimIds, ct);
        return settlements.Sum(s => s.Amount);
    }

    public async Task<decimal> GetOutstandingAmountAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _claimRepository.GetByDateRangeAsync(from, to, ct);
        var activeClaims = claims.Where(c => c.Status.Name == "Approved" || c.Status.Name == "Paid").ToList();

        var claimIds = activeClaims.Select(c => c.Id).ToList();
        var settlements = await _settlementRepository.GetByClaimIdsAsync(claimIds, ct);
        var settlementLookup = settlements.GroupBy(s => s.ClaimId).ToDictionary(g => g.Key, g => g.Sum(s => s.Amount));

        return activeClaims.Sum(c => c.PayerShare - settlementLookup.GetValueOrDefault(c.Id, 0m));
    }
}
