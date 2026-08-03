using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShifts;

public static class GetWorkShiftsQueryHandler
{
    public static async Task<Result<PagedResult<WorkShiftDto>>> HandleAsync(
        GetWorkShiftsQuery query,
        IWorkShiftRepository workShiftRepository,
        CancellationToken ct)
    {
        var paged = await workShiftRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(w => w.Adapt<WorkShiftDto>()).ToList();

        return Result.Success(new PagedResult<WorkShiftDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
