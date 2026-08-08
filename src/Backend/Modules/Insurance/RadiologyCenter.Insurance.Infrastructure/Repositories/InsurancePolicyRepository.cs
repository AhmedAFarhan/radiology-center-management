using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class InsurancePolicyRepository : BaseRepository<InsurancePolicy, Guid>, IInsurancePolicyRepository
{
    public InsurancePolicyRepository(InsuranceDbContext context) : base(context) { }

    public async Task<IReadOnlyList<InsurancePolicy>> GetActiveByPatientIdAsync(Guid patientId, CancellationToken ct = default) =>
        await DbSet
            .Where(p => p.PatientId == patientId && p.IsActive)
            .ToListAsync(ct);

    public async Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken ct = default) =>
        await DbSet.AnyAsync(p => p.PolicyNumber == policyNumber, ct);
}