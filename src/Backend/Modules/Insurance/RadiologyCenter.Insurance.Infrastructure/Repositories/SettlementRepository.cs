using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class SettlementRepository : BaseRepository<Settlement, Guid>, ISettlementRepository
{
    public SettlementRepository(InsuranceDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Settlement>> GetByClaimIdsAsync(IEnumerable<Guid> claimIds, CancellationToken ct = default)
    {
        var idList = claimIds.Distinct().ToList();
        if (idList.Count == 0)
            return [];

        return await DbSet
            .Where(s => idList.Contains(s.ClaimId))
            .ToListAsync(ct);
    }
}
