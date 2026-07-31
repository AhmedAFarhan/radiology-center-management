using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.GetStockMovements;

public static class GetStockMovementsQueryHandler
{
    public static async Task<Result<PagedResult<StockMovementDto>>> HandleAsync(
        GetStockMovementsQuery query,
        IStockMovementRepository stockMovementRepository,
        CancellationToken ct)
    {
        var paged = await stockMovementRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(m => m.Adapt<StockMovementDto>()).ToList();

        return Result.Success(new PagedResult<StockMovementDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
