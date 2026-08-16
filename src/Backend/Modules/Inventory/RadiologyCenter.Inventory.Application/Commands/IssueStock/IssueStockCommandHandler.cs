using RadiologyCenter.Inventory.Application.Localization;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Commands.IssueStock;

public static class IssueStockCommandHandler
{
    public static async Task<Result> HandleAsync(
        IssueStockCommand command,
        IItemRepository itemRepository,
        IStockBatchRepository stockBatchRepository,
        IStockMovementRepository stockMovementRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var item = await itemRepository.GetByIdAsync(command.ItemId, ct);
        if (item is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ItemNotFound, "Item", command.ItemId));

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        var batches = await stockBatchRepository.GetAvailableForItemForUpdateAsync(command.ItemId, ct);
        var available = batches.Sum(b => b.QuantityRemaining);
        if (available < command.Quantity)
            return Result.Failure(Error.Conflict(
                ErrorCodes.InsufficientStock,
                $"Only {available} units available for item '{item.Name}'."));

        var remaining = command.Quantity;
        foreach (var batch in batches)
        {
            if (remaining == 0) break;

            var take = Math.Min(batch.QuantityRemaining, remaining);
            batch.Issue(take);
            stockBatchRepository.Update(batch);

            var movement = StockMovement.Create(
                command.ItemId,
                StockMovementType.Issue,
                -take,
                batch.Id,
                reference: command.Reference,
                notes: command.Notes);
            await stockMovementRepository.AddAsync(movement, ct);

            remaining -= take;
        }

        var stockOnHand = available - command.Quantity;
        if (item.ReorderLevel > 0 && stockOnHand < item.ReorderLevel)
            item.ReportLowStock(stockOnHand);

        await transaction.CommitAsync(ct);
        return Result.Success();
    }
}