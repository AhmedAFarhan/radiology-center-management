using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateSupplier;

public static class UpdateSupplierCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateSupplierCommand command,
        ISupplierRepository supplierRepository,
        IInventoryUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(command.SupplierId, ct);
        if (supplier is null)
            return Result.Failure(Error.NotFound("Supplier", command.SupplierId));

        supplier.Update(
            command.Name,
            command.Phone,
            command.ContactPerson,
            command.Email,
            command.Address,
            command.TaxNumber,
            command.PaymentTerms);

        supplierRepository.Update(supplier);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
