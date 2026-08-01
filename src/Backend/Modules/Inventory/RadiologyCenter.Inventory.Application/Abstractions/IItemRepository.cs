using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Application.Abstractions;

public interface IItemRepository : IBaseRepository<Item, Guid>
{
    Task<IReadOnlyList<Item>> FindIncludingDeletedAsync(ISpecification<Item> spec, CancellationToken ct = default);
}
