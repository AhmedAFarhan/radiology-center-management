using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Commands.ReceivePurchaseOrder;

public static class ReceivePurchaseOrderCommandHandler
{
    public static async Task<Result> HandleAsync(
        ReceivePurchaseOrderCommand command,
        IPurchaseOrderRepository purchaseOrderRepository,
        IStockBatchRepository stockBatchRepository,
        IStockMovementRepository stockMovementRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var purchaseOrder = await purchaseOrderRepository.GetWithItemsAsync(command.PurchaseOrderId, ct);
        if (purchaseOrder is null)
            return Result.Failure(Error.NotFound("PurchaseOrder", command.PurchaseOrderId));

        try
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

            foreach (var line in command.Lines)
            {
                purchaseOrder.RecordReceipt(line.ItemId, line.Quantity);

                var orderLine = purchaseOrder.Items.First(i => i.ItemId == line.ItemId);

                var batch = StockBatch.Create(
                    line.ItemId,
                    line.LotNumber,
                    line.Quantity,
                    line.ExpiryDate,
                    purchaseOrder.SupplierId);
                await stockBatchRepository.AddAsync(batch, ct);

                var movement = StockMovement.Create(
                    line.ItemId,
                    StockMovementType.Receive,
                    line.Quantity,
                    batch.Id,
                    orderLine.UnitCost,
                    purchaseOrder.OrderNumber);
                await stockMovementRepository.AddAsync(movement, ct);
            }

            purchaseOrderRepository.Update(purchaseOrder);
            await transaction.CommitAsync(ct);

            return Result.Success();
        }
        catch (DomainException exception)
        {
            return Result.Failure(Error.Validation("InvalidReceipt", exception.Message));
        }
    }
}
