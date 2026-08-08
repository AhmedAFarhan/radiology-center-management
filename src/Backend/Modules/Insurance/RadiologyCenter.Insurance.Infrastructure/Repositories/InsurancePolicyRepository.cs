using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class InsurancePolicyRepository : BaseRepository<InsurancePolicy, Guid>, IInsurancePolicyRepository
{
    public InsurancePolicyRepository(InsuranceDbContext context) : base(context) { }

    public async Task<IReadOnlyList<InsurancePolicy>> GetActiveByPatientIdAsync(Guid patientId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await DbSet
            .Where(p => p.PatientId == patientId
                && p.Status == PolicyStatus.Active
                && (!p.EffectiveTo.HasValue || p.EffectiveTo.Value >= now))
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken ct = default) =>
        await DbSet.AnyAsync(p => p.PolicyNumber == policyNumber, ct);

    public async Task<IReadOnlyList<InsurancePolicy>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new List<InsurancePolicy>();

        return await DbSet.Where(p => idList.Contains(p.Id)).ToListAsync(ct);
    }
}