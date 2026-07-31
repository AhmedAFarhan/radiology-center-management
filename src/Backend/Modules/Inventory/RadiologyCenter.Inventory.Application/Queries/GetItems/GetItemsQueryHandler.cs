using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.GetItems;

public static class GetItemsQueryHandler
{
    public static async Task<Result<PagedResult<ItemDto>>> HandleAsync(
        GetItemsQuery query,
        IItemRepository itemRepository,
        CancellationToken ct)
    {
        var paged = await itemRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(i => i.Adapt<ItemDto>()).ToList();

        return Result.Success(new PagedResult<ItemDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
