using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.SetEquipmentStatus;

public static class SetEquipmentStatusCommandHandler
{
    public static async Task<Result> HandleAsync(
        SetEquipmentStatusCommand command,
        IEquipmentRepository equipmentRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var equipment = await equipmentRepository.GetByIdAsync(command.EquipmentId, ct);
        if (equipment is null)
            return Result.Failure(Error.NotFound(ErrorCodes.EquipmentNotFound, "Equipment", command.EquipmentId));

        var status = EquipmentStatus.FromName<EquipmentStatus>(command.Status);

        equipment.SetStatus(status);
        equipmentRepository.Update(equipment);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
