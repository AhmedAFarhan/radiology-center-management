using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Localhost.Extensions;

public sealed class InsuranceAnalyticsDataSource : IInsuranceAnalyticsDataSource
{
    private readonly InsuranceDbContext _db;
    private readonly IInsuranceDirectory _directory;

    public InsuranceAnalyticsDataSource(InsuranceDbContext db, IInsuranceDirectory directory)
    {
        _db = db;
        _directory = directory;
    }

    public async Task<IReadOnlyList<InsuranceClaimRowDto>> GetClaimRowsAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _db.Claims
            .Where(c => c.CreatedAt >= from && c.CreatedAt < to)
            .ToListAsync(ct);

        if (claims.Count == 0)
            return [];

        var policyIds = claims.Select(c => c.PolicyId).Distinct().ToList();
        var policies = await _db.InsurancePolicies.Where(p => policyIds.Contains(p.Id)).ToListAsync(ct);
        var policyLookup = policies.ToDictionary(p => p.Id);

        var companyIds = policies.Select(p => p.CompanyId).Distinct().ToList();
        var companies = await _db.InsuranceCompanies.Where(c => companyIds.Contains(c.Id)).ToListAsync(ct);
        var companyLookup = companies.ToDictionary(c => c.Id);

        var patientIds = claims.Select(c => c.PatientId).Distinct().ToList();
        var patientNames = await _directory.ResolvePatientNamesAsync(patientIds, ct);

        var claimIds = claims.Select(c => c.Id).ToList();
        var settlements = await _db.Settlements.Where(s => claimIds.Contains(s.ClaimId)).ToListAsync(ct);
        var settlementLookup = settlements.GroupBy(s => s.ClaimId).ToDictionary(g => g.Key, g => g.Sum(s => s.Amount));

        return claims.Select(c =>
        {
            var policy = policyLookup.GetValueOrDefault(c.PolicyId);
            var company = policy is not null ? companyLookup.GetValueOrDefault(policy.CompanyId) : null;
            var settled = settlementLookup.GetValueOrDefault(c.Id, 0m);
            var name = patientNames.TryGetValue(c.PatientId, out var n) ? n : "";

            return new InsuranceClaimRowDto(
                c.Id,
                name,
                company?.Name ?? "",
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
        => await _db.Claims.CountAsync(c => c.CreatedAt >= from && c.CreatedAt < to, ct);

    public async Task<int> GetClaimsByStatusAsync(string status, DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.Claims.CountAsync(c => c.Status.Name == status && c.CreatedAt >= from && c.CreatedAt < to, ct);

    public async Task<decimal> GetTotalBilledAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.Claims.Where(c => c.CreatedAt >= from && c.CreatedAt < to).SumAsync(c => c.BilledAmount, ct);

    public async Task<decimal> GetTotalPayerShareAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.Claims.Where(c => c.CreatedAt >= from && c.CreatedAt < to).SumAsync(c => c.PayerShare, ct);

    public async Task<decimal> GetTotalPatientShareAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.Claims.Where(c => c.CreatedAt >= from && c.CreatedAt < to).SumAsync(c => c.PatientShare, ct);

    public async Task<decimal> GetTotalSettledAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claimIds = await _db.Claims
            .Where(c => c.CreatedAt >= from && c.CreatedAt < to)
            .Select(c => c.Id)
            .ToListAsync(ct);
        return await _db.Settlements
            .Where(s => claimIds.Contains(s.ClaimId))
            .SumAsync(s => s.Amount, ct);
    }

    public async Task<decimal> GetOutstandingAmountAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var claims = await _db.Claims
            .Where(c => c.CreatedAt >= from && c.CreatedAt < to && (c.Status.Name == "Approved" || c.Status.Name == "Paid"))
            .ToListAsync(ct);

        var claimIds = claims.Select(c => c.Id).ToList();
        var settlements = await _db.Settlements
            .Where(s => claimIds.Contains(s.ClaimId))
            .GroupBy(s => s.ClaimId)
            .Select(g => new { ClaimId = g.Key, Total = g.Sum(s => s.Amount) })
            .ToListAsync(ct);
        var settlementLookup = settlements.ToDictionary(x => x.ClaimId, x => x.Total);

        return claims.Sum(c => c.PayerShare - settlementLookup.GetValueOrDefault(c.Id, 0m));
    }
}
