using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Infrastructure.Persistence;

namespace RadiologyCenter.Inventory.Infrastructure.Repositories;

public class ItemRepository : BaseRepository<Item, Guid>, IItemRepository
{
    public ItemRepository(InventoryDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Item>> FindIncludingDeletedAsync(ISpecification<Item> spec, CancellationToken ct = default)
    {
        var query = DbSet.IgnoreQueryFilters().AsQueryable();
        return await SpecificationEvaluator<Item>.GetQuery(query, spec).ToListAsync(ct);
    }
}
