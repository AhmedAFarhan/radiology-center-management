using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetEquipmentById;

public static class GetEquipmentByIdQueryHandler
{
    public static async Task<Result<EquipmentDto>> HandleAsync(
        GetEquipmentByIdQuery query,
        IEquipmentRepository equipmentRepository,
        CancellationToken ct)
    {
        var equipment = await equipmentRepository.GetByIdAsync(query.Id, ct);
        if (equipment is null)
            return Result.Failure<EquipmentDto>(Error.NotFound("Equipment", query.Id));

        return Result.Success(equipment.Adapt<EquipmentDto>());
    }
}
