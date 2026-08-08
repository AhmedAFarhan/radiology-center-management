using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class ClaimRepository : BaseRepository<Claim, Guid>, IClaimRepository
{
    public ClaimRepository(InsuranceDbContext context) : base(context) { }

    public override async Task<Claim?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(c => c.Settlements)
            .Include(c => c.Rejections)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Claim?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default) =>
        await DbSet
            .Include(c => c.Settlements)
            .Include(c => c.Rejections)
            .FirstOrDefaultAsync(c => c.ExaminationId == examinationId, ct);
}