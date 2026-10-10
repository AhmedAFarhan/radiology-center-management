using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Inventory.Application.Localization;
using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.CancelPurchaseOrder;

public static class CancelPurchaseOrderCommandHandler
{
    public static async Task<Result> HandleAsync(
        CancelPurchaseOrderCommand command,
        IPurchaseOrderRepository purchaseOrderRepository,
        IInventoryUnitOfWork unitOfWork,
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, new[] { "inventory:purchaseorders" }, ct);
        var purchaseOrder = await purchaseOrderRepository.GetWithItemsAsync(command.PurchaseOrderId, ct);
        if (purchaseOrder is null)
            return Result.Failure(Error.NotFound(ErrorCodes.PurchaseOrderNotFound, "PurchaseOrder", command.PurchaseOrderId));

        purchaseOrder.Cancel();
        purchaseOrderRepository.Update(purchaseOrder);
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Success();
    }
}
