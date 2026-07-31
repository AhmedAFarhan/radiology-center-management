using Mapster;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.GetItemById;

public static class GetItemByIdQueryHandler
{
    public static async Task<Result<ItemDto>> HandleAsync(
        GetItemByIdQuery query,
        IItemRepository itemRepository,
        CancellationToken ct)
    {
        var item = await itemRepository.GetByIdAsync(query.Id, ct);
        if (item is null)
            return Result.Failure<ItemDto>(Error.NotFound("Item", query.Id));

        return Result.Success(item.Adapt<ItemDto>());
    }
}
