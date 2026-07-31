using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Infrastructure.Persistence;

namespace RadiologyCenter.Inventory.Infrastructure.Repositories;

public class SupplierRepository : BaseRepository<Supplier, Guid>, ISupplierRepository
{
    public SupplierRepository(InventoryDbContext context) : base(context) { }
}
