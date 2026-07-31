using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Infrastructure.Persistence;

public class InventoryUnitOfWork : UnitOfWork<InventoryDbContext>, IInventoryUnitOfWork
{
    public InventoryUnitOfWork(InventoryDbContext context, IDomainEventDispatcher eventDispatcher)
        : base(context, eventDispatcher)
    {
    }
}
