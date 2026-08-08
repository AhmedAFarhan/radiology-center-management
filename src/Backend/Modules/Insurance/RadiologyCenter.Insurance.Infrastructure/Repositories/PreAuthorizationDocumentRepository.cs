using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class PreAuthorizationDocumentRepository : BaseRepository<PreAuthorizationDocument, Guid>, IPreAuthorizationDocumentRepository
{
    public PreAuthorizationDocumentRepository(InsuranceDbContext context) : base(context) { }

    public async Task<IReadOnlyList<PreAuthorizationDocument>> GetByPreAuthorizationIdAsync(Guid preAuthorizationId, CancellationToken ct = default) =>
        await DbSet
            .Where(d => d.PreAuthorizationId == preAuthorizationId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid preAuthorizationId, DocumentType type, CancellationToken ct = default) =>
        await DbSet.AnyAsync(d => d.PreAuthorizationId == preAuthorizationId && d.Type == type, ct);
}