using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.GetSuppliers;

public static class GetSuppliersQueryHandler
{
    public static async Task<Result<PagedResult<SupplierDto>>> HandleAsync(
        GetSuppliersQuery query,
        ISupplierRepository supplierRepository,
        CancellationToken ct)
    {
        var paged = await supplierRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(s => s.Adapt<SupplierDto>()).ToList();

        return Result.Success(new PagedResult<SupplierDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
