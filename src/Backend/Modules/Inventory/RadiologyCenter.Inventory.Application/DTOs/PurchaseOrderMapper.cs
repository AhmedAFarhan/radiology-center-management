using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Application.DTOs;

public static class PurchaseOrderMapper
{
    public static PurchaseOrderDto Map(
        PurchaseOrder purchaseOrder,
        IReadOnlyDictionary<Guid, string> itemNames,
        string supplierName) =>
        new(
            purchaseOrder.Id,
            purchaseOrder.OrderNumber,
            purchaseOrder.SupplierId,
            supplierName,
            purchaseOrder.Status.LocalizedName(),
            purchaseOrder.ExpectedDeliveryAt,
            purchaseOrder.ReceivedAt,
            purchaseOrder.Notes,
            purchaseOrder.Items
                .Select(i => new PurchaseOrderItemDto(
                    i.Id,
                    i.ItemId,
                    itemNames.TryGetValue(i.ItemId, out var name) ? name : string.Empty,
                    i.QuantityOrdered,
                    i.UnitCost,
                    i.QuantityReceived))
                .ToList());

    public static async Task<IReadOnlyDictionary<Guid, string>> LoadItemNamesAsync(
        IEnumerable<Guid> itemIds,
        IItemRepository itemRepository,
        CancellationToken ct)
    {
        var ids = itemIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, string>();

        var spec = new DynamicSpecification<Item>();
        spec.AddCriteria(i => ids.Contains(i.Id));
        var items = await itemRepository.FindAsync(spec, ct);
        return items.ToDictionary(i => i.Id, i => i.Name);
    }
}
