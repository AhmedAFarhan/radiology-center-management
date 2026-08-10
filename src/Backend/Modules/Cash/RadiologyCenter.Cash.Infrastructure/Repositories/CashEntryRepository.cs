using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Domain.Entities;
using RadiologyCenter.Cash.Domain.Enumerations;
using RadiologyCenter.Cash.Infrastructure.Persistence;

namespace RadiologyCenter.Cash.Infrastructure.Repositories;

public class CashEntryRepository : BaseRepository<CashEntry, Guid>, ICashEntryRepository
{
    public CashEntryRepository(CashDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CashEntry>> GetBySessionAsync(Guid cashSessionId, CancellationToken ct = default) =>
        await DbSet
            .Where(e => e.CashSessionId == cashSessionId)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyDictionary<Guid, (decimal Movement, int Count)>> GetSessionMovementsAsync(
        IEnumerable<Guid> sessionIds,
        CancellationToken ct = default)
    {
        var idList = sessionIds.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, (decimal Movement, int Count)>();

        var rows = await DbSet
            .Where(e => idList.Contains(e.CashSessionId))
            .GroupBy(e => e.CashSessionId)
            .Select(g => new
            {
                SessionId = g.Key,
                Movement = g.Sum(e => e.Direction == CashEntryDirection.In ? e.Amount : -e.Amount),
                Count = g.Count()
            })
            .ToListAsync(ct);

        return rows.ToDictionary(r => r.SessionId, r => (r.Movement, r.Count));
    }
}