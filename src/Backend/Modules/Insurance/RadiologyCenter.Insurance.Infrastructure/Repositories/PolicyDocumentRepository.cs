using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class PolicyDocumentRepository : BaseRepository<PolicyDocument, Guid>, IPolicyDocumentRepository
{
    public PolicyDocumentRepository(InsuranceDbContext context) : base(context) { }

    public async Task<IReadOnlyList<PolicyDocument>> GetByPolicyIdAsync(Guid policyId, CancellationToken ct = default) =>
        await DbSet
            .Where(d => d.PolicyId == policyId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid policyId, DocumentType type, CancellationToken ct = default) =>
        await DbSet.AnyAsync(d => d.PolicyId == policyId && d.Type == type, ct);
}