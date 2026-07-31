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
        CancellationToken ct)
    {
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

        return Result.Success(supplier.Adapt<SupplierDto>());
    }
}
