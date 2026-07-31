using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.PlacePurchaseOrder;

public static class PlacePurchaseOrderCommandHandler
{
    public static async Task<Result> HandleAsync(
        PlacePurchaseOrderCommand command,
        IPurchaseOrderRepository purchaseOrderRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var purchaseOrder = await purchaseOrderRepository.GetWithItemsAsync(command.PurchaseOrderId, ct);
        if (purchaseOrder is null)
            return Result.Failure(Error.NotFound("PurchaseOrder", command.PurchaseOrderId));

        purchaseOrder.Place();
        purchaseOrderRepository.Update(purchaseOrder);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
