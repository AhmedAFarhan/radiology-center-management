using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.DeleteItem;

public static class DeleteItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteItemCommand command,
        IItemRepository itemRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var item = await itemRepository.GetByIdAsync(command.ItemId, ct);
        if (item is null)
            return Result.Failure(Error.NotFound("Item", command.ItemId));

        itemRepository.Remove(item);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
