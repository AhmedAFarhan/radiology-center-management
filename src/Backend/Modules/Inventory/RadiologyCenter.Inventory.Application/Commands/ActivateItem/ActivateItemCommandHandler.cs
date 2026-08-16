using RadiologyCenter.Inventory.Application.Localization;
using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.ActivateItem;

public static class ActivateItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateItemCommand command,
        IItemRepository itemRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var item = await itemRepository.GetByIdAsync(command.ItemId, ct);
        if (item is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ItemNotFound, "Item", command.ItemId));

        item.Activate();
        itemRepository.Update(item);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
