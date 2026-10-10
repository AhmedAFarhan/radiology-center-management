using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Inventory.Application.Localization;
using RadiologyCenter.Inventory.Application.Abstractions;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateSupplier;

public static class UpdateSupplierCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateSupplierCommand command,
        ISupplierRepository supplierRepository,
        IInventoryUnitOfWork unitOfWork,
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, ["inventory:suppliers"], ct);
        var duplicate = await supplierRepository.FindSingleAsync(new DynamicSpecification<Supplier>(s => s.Phone == command.Phone && s.Id != command.SupplierId), ct);
        if (duplicate is not null)
            return Result.Failure(Error.Conflict("Inventory.SupplierPhoneExists", "A supplier with this phone number already exists."));

        var supplier = await supplierRepository.GetByIdAsync(command.SupplierId, ct);
        if (supplier is null)
            return Result.Failure(Error.NotFound(ErrorCodes.SupplierNotFound, "Supplier", command.SupplierId));

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
        await transaction.CommitAsync(ct);
        return Result.Success();
    }
}
