using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Cash.Application.Abstractions;

public interface ICashEntryRepository : IBaseRepository<CashEntry, Guid>
{
    Task<IReadOnlyList<CashEntry>> GetBySessionAsync(Guid cashSessionId, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, (decimal Movement, int Count)>> GetSessionMovementsAsync(IEnumerable<Guid> sessionIds, CancellationToken ct = default);
}