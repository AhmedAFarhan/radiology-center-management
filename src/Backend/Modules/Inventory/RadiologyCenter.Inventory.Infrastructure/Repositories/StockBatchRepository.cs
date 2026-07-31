using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Infrastructure.Persistence;

namespace RadiologyCenter.Inventory.Infrastructure.Repositories;

public class StockBatchRepository : BaseRepository<StockBatch, Guid>, IStockBatchRepository
{
    public StockBatchRepository(InventoryDbContext context) : base(context) { }

    public async Task<IReadOnlyList<StockBatch>> GetAvailableForItemAsync(Guid itemId, CancellationToken ct = default) =>
        await DbSet.AsNoTracking()
            .Where(b => b.ItemId == itemId && b.QuantityRemaining > 0)
            .OrderBy(b => b.ExpiryDate ?? DateTime.MaxValue)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<StockBatch>> GetForItemAsync(Guid itemId, CancellationToken ct = default) =>
        await DbSet.AsNoTracking()
            .Where(b => b.ItemId == itemId)
            .OrderBy(b => b.ExpiryDate ?? DateTime.MaxValue)
            .ToListAsync(ct);
}
