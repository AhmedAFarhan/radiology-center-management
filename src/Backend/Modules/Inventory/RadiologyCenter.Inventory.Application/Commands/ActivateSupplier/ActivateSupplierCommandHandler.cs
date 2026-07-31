using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.ActivateSupplier;

public static class ActivateSupplierCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateSupplierCommand command,
        ISupplierRepository supplierRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(command.SupplierId, ct);
        if (supplier is null)
            return Result.Failure(Error.NotFound("Supplier", command.SupplierId));

        supplier.Activate();
        supplierRepository.Update(supplier);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
