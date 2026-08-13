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

    public async Task<Claim?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default)
    {
        var claim = await DbSet
            .FromSqlInterpolated($"SELECT * FROM [Insurance].[Claims] WITH (UPDLOCK, ROWLOCK) WHERE [Id] = {id}")
            .SingleOrDefaultAsync(ct);

        if (claim is null)
            return null;

        await DbSet.Entry(claim).Collection(c => c.Settlements).LoadAsync(ct);
        await DbSet.Entry(claim).Collection(c => c.Rejections).LoadAsync(ct);
        return claim;
    }

    public async Task<Claim?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default) =>
        await DbSet
            .Include(c => c.Settlements)
            .Include(c => c.Rejections)
            .FirstOrDefaultAsync(c => c.ExaminationId == examinationId, ct);
}