using RadiologyCenter.Inventory.Application.Localization;
using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.DeleteSupplier;

public static class DeleteSupplierCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteSupplierCommand command,
        ISupplierRepository supplierRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(command.SupplierId, ct);
        if (supplier is null)
            return Result.Failure(Error.NotFound(ErrorCodes.SupplierNotFound, "Supplier", command.SupplierId));

        supplierRepository.Remove(supplier);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
