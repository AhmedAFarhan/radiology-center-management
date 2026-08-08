using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class PreAuthorizationRepository : BaseRepository<PreAuthorization, Guid>, IPreAuthorizationRepository
{
    public PreAuthorizationRepository(InsuranceDbContext context) : base(context) { }

    public override async Task<PreAuthorization?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(p => p.Documents)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PreAuthorization?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default) =>
        await DbSet
            .Include(p => p.Documents)
            .FirstOrDefaultAsync(p => p.ExaminationId == examinationId, ct);
}