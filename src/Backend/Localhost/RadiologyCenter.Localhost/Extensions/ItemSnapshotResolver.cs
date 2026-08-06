using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.ValueObjects;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Domain.Enumerations;
using RadiologyCenter.Inventory.Infrastructure.Persistence;

namespace RadiologyCenter.Localhost.Extensions;

public sealed class ItemSnapshotResolver : IItemSnapshotResolver
{
    private readonly IItemRepository _itemRepository;
    private readonly InventoryDbContext _inventory;

    public ItemSnapshotResolver(IItemRepository itemRepository, InventoryDbContext inventory)
    {
        _itemRepository = itemRepository;
        _inventory = inventory;
    }

    public async Task<IReadOnlyDictionary<Guid, ItemSnapshot>> ResolveAsync(IEnumerable<Guid> itemIds, CancellationToken ct)
    {
        var ids = itemIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, ItemSnapshot>();

        var costs = await ComputeWeightedAverageCostsAsync(ids, ct);

        var spec = new DynamicSpecification<Item>();
        spec.AddCriteria(i => ids.Contains(i.Id));
        var items = await _itemRepository.FindIncludingDeletedAsync(spec, ct);

        return items.ToDictionary(
            i => i.Id,
            i => new ItemSnapshot(i.Id, i.Name, i.Category.Value,
                costs.TryGetValue(i.Id, out var cost) ? Math.Round(cost, 2) : 0m));
    }

    private async Task<IReadOnlyDictionary<Guid, decimal>> ComputeWeightedAverageCostsAsync(IReadOnlyList<Guid> itemIds, CancellationToken ct)
    {
        var aggregates = await _inventory.StockMovements
            .Where(m => itemIds.Contains(m.ItemId)
                        && m.MovementType == StockMovementType.Receive
                        && m.UnitCost != null)
            .GroupBy(m => m.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                Quantity = (decimal)g.Sum(m => m.Quantity),
                Value = g.Sum(m => m.Quantity * m.UnitCost!.Value)
            })
            .ToListAsync(ct);

        return aggregates
            .Where(a => a.Quantity > 0)
            .ToDictionary(a => a.ItemId, a => a.Value / a.Quantity);
    }
}