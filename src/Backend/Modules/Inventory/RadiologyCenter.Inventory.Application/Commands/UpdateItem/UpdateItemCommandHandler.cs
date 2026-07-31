using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateItem;

public static class UpdateItemCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateItemCommand command,
        IItemRepository itemRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var item = await itemRepository.GetByIdAsync(command.ItemId, ct);
        if (item is null)
            return Result.Failure(Error.NotFound("Item", command.ItemId));

        var category = ItemCategory.FromName<ItemCategory>(command.Category);
        var unit = UnitType.FromName<UnitType>(command.Unit);

        item.Update(
            command.Name,
            category,
            unit,
            command.Brand,
            command.ReorderLevel,
            command.ReorderQuantity,
            command.LotTracked,
            command.StorageInstructions);

        itemRepository.Update(item);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
