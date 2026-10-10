using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using Mapster;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Commands.CreateSupplier;

public static class CreateSupplierCommandHandler
{
    public static async Task<Result<SupplierDto>> HandleAsync(
        CreateSupplierCommand command,
        ISupplierRepository supplierRepository,
        IInventoryUnitOfWork unitOfWork,
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, ["inventory:suppliers"], ct);
        var duplicate = await supplierRepository.FindSingleAsync(new DynamicSpecification<Supplier>(s => s.Phone == command.Phone), ct);
        if (duplicate is not null)
            return Result.Failure<SupplierDto>(Error.Conflict("Inventory.SupplierPhoneExists", "A supplier with this phone number already exists."));

        var supplier = Supplier.Create(
            command.Name,
            command.Phone,
            command.ContactPerson,
            command.Email,
            command.Address,
            command.TaxNumber,
            command.PaymentTerms);

        await supplierRepository.AddAsync(supplier, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Success(supplier.Adapt<SupplierDto>());
    }
}
