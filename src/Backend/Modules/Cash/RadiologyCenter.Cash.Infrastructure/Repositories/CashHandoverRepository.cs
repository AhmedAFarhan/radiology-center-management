using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Domain.Entities;
using RadiologyCenter.Cash.Infrastructure.Persistence;

namespace RadiologyCenter.Cash.Infrastructure.Repositories;

public class CashHandoverRepository : BaseRepository<CashHandover, Guid>, ICashHandoverRepository
{
    public CashHandoverRepository(CashDbContext context) : base(context) { }

    public async Task<CashHandover?> GetBySessionAsync(Guid cashSessionId, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(h => h.CashSessionId == cashSessionId, ct);

    public async Task<IReadOnlyList<CashHandover>> GetByClosedByUserAsync(Guid userId, CancellationToken ct = default) =>
        await DbSet
            .Where(h => h.ClosedByUserId == userId)
            .OrderByDescending(h => h.ClosedAt)
            .ToListAsync(ct);
}