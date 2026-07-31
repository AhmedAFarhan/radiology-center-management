using Mapster;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Commands.CreateItem;

public static class CreateItemCommandHandler
{
    public static async Task<Result<ItemDto>> HandleAsync(
        CreateItemCommand command,
        IItemRepository itemRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var category = ItemCategory.FromName<ItemCategory>(command.Category);
        var unit = UnitType.FromName<UnitType>(command.Unit);

        var item = Item.Create(
            command.Name,
            category,
            unit,
            command.Brand,
            command.ReorderLevel,
            command.ReorderQuantity,
            command.LotTracked,
            command.StorageInstructions);

        await itemRepository.AddAsync(item, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(item.Adapt<ItemDto>());
    }
}
