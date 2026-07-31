using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.DeactivateSupplier;

public static class DeactivateSupplierCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateSupplierCommand command,
        ISupplierRepository supplierRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(command.SupplierId, ct);
        if (supplier is null)
            return Result.Failure(Error.NotFound("Supplier", command.SupplierId));

        supplier.Deactivate();
        supplierRepository.Update(supplier);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
