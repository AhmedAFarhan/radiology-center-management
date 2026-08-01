using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.ValueObjects;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Localhost.Extensions;

public sealed class ItemSnapshotResolver : IItemSnapshotResolver
{
    private readonly IItemRepository _itemRepository;

    public ItemSnapshotResolver(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<IReadOnlyDictionary<Guid, ItemSnapshot>> ResolveAsync(IEnumerable<Guid> itemIds, CancellationToken ct)
    {
        var ids = itemIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, ItemSnapshot>();

        var spec = new DynamicSpecification<Item>();
        spec.AddCriteria(i => ids.Contains(i.Id));
        var items = await _itemRepository.FindAsync(spec, ct);

        return items.ToDictionary(i => i.Id, i => new ItemSnapshot(i.Id, i.Name, i.Category.Value));
    }
}
