using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.GetPurchaseOrders;

public static class GetPurchaseOrdersQueryHandler
{
    public static async Task<Result<PagedResult<PurchaseOrderDto>>> HandleAsync(
        GetPurchaseOrdersQuery query,
        IPurchaseOrderRepository purchaseOrderRepository,
        ISupplierRepository supplierRepository,
        CancellationToken ct)
    {
        var paged = await purchaseOrderRepository.GetPagedAsync(query.Request, ct);

        var supplierIds = paged.Items.Select(po => po.SupplierId).Distinct().ToList();
        var supplierNames = new Dictionary<Guid, string>();
        if (supplierIds.Count > 0)
        {
            var spec = new DynamicSpecification<Supplier>();
            spec.AddCriteria(s => supplierIds.Contains(s.Id));
            var suppliers = await supplierRepository.FindAsync(spec, ct);
            supplierNames = suppliers.ToDictionary(s => s.Id, s => s.Name);
        }

        var dtos = paged.Items
            .Select(po => PurchaseOrderMapper.Map(
                po,
                new Dictionary<Guid, string>(),
                supplierNames.GetValueOrDefault(po.SupplierId) ?? string.Empty))
            .ToList();

        return Result.Success(new PagedResult<PurchaseOrderDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
