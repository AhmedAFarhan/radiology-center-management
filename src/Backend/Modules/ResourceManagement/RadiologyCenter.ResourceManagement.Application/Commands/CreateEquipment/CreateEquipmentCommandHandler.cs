using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateEquipment;

public static class CreateEquipmentCommandHandler
{
    public static async Task<Result<EquipmentDto>> HandleAsync(
        CreateEquipmentCommand command,
        IEquipmentRepository equipmentRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var modality = EquipmentModality.FromName<EquipmentModality>(command.Modality);

        var equipment = Equipment.Create(
            command.Name,
            modality,
            command.SerialNumber,
            command.PurchaseDate);

        await equipmentRepository.AddAsync(equipment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(equipment.Adapt<EquipmentDto>());
    }
}
