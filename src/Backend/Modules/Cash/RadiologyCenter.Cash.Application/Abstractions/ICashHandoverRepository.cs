using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Cash.Application.Abstractions;

public interface ICashHandoverRepository : IBaseRepository<CashHandover, Guid>
{
    Task<CashHandover?> GetBySessionAsync(Guid cashSessionId, CancellationToken ct = default);
    Task<IReadOnlyList<CashHandover>> GetByClosedByUserAsync(Guid userId, CancellationToken ct = default);
}