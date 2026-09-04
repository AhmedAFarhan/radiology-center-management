using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Examinations.Application.Abstractions;
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

    public async Task<IReadOnlyDictionary<Guid, decimal>> ResolveAsync(IEnumerable<Guid> itemIds, CancellationToken ct)
    {
        var ids = itemIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, decimal>();

        var costs = await ComputeWeightedAverageCostsAsync(ids, ct);

        return costs;
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
            .ToDictionary(a => a.ItemId, a => Math.Round(a.Value / a.Quantity, 2));
    }
}
