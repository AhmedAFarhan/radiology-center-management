using Mapster;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.GetSupplierById;

public static class GetSupplierByIdQueryHandler
{
    public static async Task<Result<SupplierDto>> HandleAsync(
        GetSupplierByIdQuery query,
        ISupplierRepository supplierRepository,
        CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(query.Id, ct);
        if (supplier is null)
            return Result.Failure<SupplierDto>(Error.NotFound("Supplier", query.Id));

        return Result.Success(supplier.Adapt<SupplierDto>());
    }
}
