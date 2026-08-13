using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Application.Abstractions;

public interface IStockBatchRepository : IBaseRepository<StockBatch, Guid>
{
    Task<IReadOnlyList<StockBatch>> GetAvailableForItemAsync(Guid itemId, CancellationToken ct = default);
    Task<IReadOnlyList<StockBatch>> GetAvailableForItemForUpdateAsync(Guid itemId, CancellationToken ct = default);
    Task<IReadOnlyList<StockBatch>> GetForItemAsync(Guid itemId, CancellationToken ct = default);
}
