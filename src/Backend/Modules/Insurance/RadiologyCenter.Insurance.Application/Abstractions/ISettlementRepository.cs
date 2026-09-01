using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Abstractions;

public interface ISettlementRepository : IBaseRepository<Settlement, Guid>
{
    Task<IReadOnlyList<Settlement>> GetByClaimIdsAsync(IEnumerable<Guid> claimIds, CancellationToken ct = default);
}
