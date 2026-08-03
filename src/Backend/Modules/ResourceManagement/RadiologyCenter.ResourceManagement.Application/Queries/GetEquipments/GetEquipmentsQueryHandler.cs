using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetEquipments;

public static class GetEquipmentsQueryHandler
{
    public static async Task<Result<PagedResult<EquipmentDto>>> HandleAsync(
        GetEquipmentsQuery query,
        IEquipmentRepository equipmentRepository,
        CancellationToken ct)
    {
        var paged = await equipmentRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(e => e.Adapt<EquipmentDto>()).ToList();

        return Result.Success(new PagedResult<EquipmentDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
