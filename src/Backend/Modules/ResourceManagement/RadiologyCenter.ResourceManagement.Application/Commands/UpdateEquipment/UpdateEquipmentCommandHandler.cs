using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateEquipment;

public static class UpdateEquipmentCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateEquipmentCommand command,
        IEquipmentRepository equipmentRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var equipment = await equipmentRepository.GetByIdAsync(command.EquipmentId, ct);
        if (equipment is null)
            return Result.Failure(Error.NotFound("Equipment", command.EquipmentId));

        var modality = EquipmentModality.FromName<EquipmentModality>(command.Modality);

        equipment.Update(
            command.Name,
            modality,
            command.SerialNumber,
            command.PurchaseDate);

        equipmentRepository.Update(equipment);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
