using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.GetPurchaseOrderById;

public static class GetPurchaseOrderByIdQueryHandler
{
    public static async Task<Result<PurchaseOrderDto>> HandleAsync(
        GetPurchaseOrderByIdQuery query,
        IPurchaseOrderRepository purchaseOrderRepository,
        IItemRepository itemRepository,
        ISupplierRepository supplierRepository,
        CancellationToken ct)
    {
        var purchaseOrder = await purchaseOrderRepository.GetWithItemsAsync(query.Id, ct);
        if (purchaseOrder is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("PurchaseOrder", query.Id));

        var itemNames = await PurchaseOrderMapper.LoadItemNamesAsync(
            purchaseOrder.Items.Select(i => i.ItemId),
            itemRepository,
            ct);
        var supplier = await supplierRepository.GetByIdAsync(purchaseOrder.SupplierId, ct);

        return Result.Success(PurchaseOrderMapper.Map(
            purchaseOrder,
            itemNames,
            supplier?.Name ?? string.Empty));
    }
}
