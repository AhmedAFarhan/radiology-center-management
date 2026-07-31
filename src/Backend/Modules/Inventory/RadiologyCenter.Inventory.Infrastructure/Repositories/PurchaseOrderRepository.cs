using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Infrastructure.Persistence;

namespace RadiologyCenter.Inventory.Infrastructure.Repositories;

public class PurchaseOrderRepository : BaseRepository<PurchaseOrder, Guid>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(InventoryDbContext context) : base(context) { }

    public async Task<PurchaseOrder?> GetWithItemsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
}
