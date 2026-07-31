using Mapster;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.GetItemStock;

public static class GetItemStockQueryHandler
{
    public static async Task<Result<ItemStockDto>> HandleAsync(
        GetItemStockQuery query,
        IItemRepository itemRepository,
        IStockBatchRepository stockBatchRepository,
        CancellationToken ct)
    {
        var item = await itemRepository.GetByIdAsync(query.ItemId, ct);
        if (item is null)
            return Result.Failure<ItemStockDto>(Error.NotFound("Item", query.ItemId));

        var batches = await stockBatchRepository.GetForItemAsync(query.ItemId, ct);
        var stockOnHand = batches.Sum(b => b.QuantityRemaining);

        return Result.Success(new ItemStockDto(
            item.Id,
            item.Name,
            stockOnHand,
            batches.Select(b => b.Adapt<StockBatchDto>()).ToList()));
    }
}
