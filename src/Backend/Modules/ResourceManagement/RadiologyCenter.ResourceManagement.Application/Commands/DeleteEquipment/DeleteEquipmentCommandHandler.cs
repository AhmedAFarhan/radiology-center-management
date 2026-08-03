using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteEquipment;

public static class DeleteEquipmentCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteEquipmentCommand command,
        IEquipmentRepository equipmentRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var equipment = await equipmentRepository.GetByIdAsync(command.EquipmentId, ct);
        if (equipment is null)
            return Result.Failure(Error.NotFound("Equipment", command.EquipmentId));

        equipmentRepository.Remove(equipment);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
