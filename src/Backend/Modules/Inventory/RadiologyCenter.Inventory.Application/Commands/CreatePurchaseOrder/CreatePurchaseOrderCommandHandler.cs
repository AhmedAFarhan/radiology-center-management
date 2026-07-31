using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Commands.CreatePurchaseOrder;

public static class CreatePurchaseOrderCommandHandler
{
    public static async Task<Result<PurchaseOrderDto>> HandleAsync(
        CreatePurchaseOrderCommand command,
        IPurchaseOrderRepository purchaseOrderRepository,
        IItemRepository itemRepository,
        ISupplierRepository supplierRepository,
        IOrderNumberGenerator orderNumberGenerator,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var orderNumber = await orderNumberGenerator.GenerateNextAsync(ct);

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        var purchaseOrder = PurchaseOrder.Create(
            orderNumber,
            command.SupplierId,
            command.ExpectedDeliveryAt,
            command.Notes);

        foreach (var line in command.Items)
            purchaseOrder.AddItem(line.ItemId, line.QuantityOrdered, line.UnitCost);

        await purchaseOrderRepository.AddAsync(purchaseOrder, ct);

        var itemNames = await PurchaseOrderMapper.LoadItemNamesAsync(
            purchaseOrder.Items.Select(i => i.ItemId),
            itemRepository,
            ct);
        var supplier = await supplierRepository.GetByIdAsync(purchaseOrder.SupplierId, ct);

        await transaction.CommitAsync(ct);

        return Result.Success(PurchaseOrderMapper.Map(
            purchaseOrder,
            itemNames,
            supplier?.Name ?? string.Empty));
    }
}
