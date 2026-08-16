using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.DeactivateEquipment;

public static class DeactivateEquipmentCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateEquipmentCommand command,
        IEquipmentRepository equipmentRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var equipment = await equipmentRepository.GetByIdAsync(command.EquipmentId, ct);
        if (equipment is null)
            return Result.Failure(Error.NotFound(ErrorCodes.EquipmentNotFound, "Equipment", command.EquipmentId));

        equipment.Deactivate();
        equipmentRepository.Update(equipment);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
