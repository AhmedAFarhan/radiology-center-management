using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Infrastructure.Persistence;

namespace RadiologyCenter.Inventory.Infrastructure.Repositories;

public class StockMovementRepository : BaseRepository<StockMovement, Guid>, IStockMovementRepository
{
    public StockMovementRepository(InventoryDbContext context) : base(context) { }
}
