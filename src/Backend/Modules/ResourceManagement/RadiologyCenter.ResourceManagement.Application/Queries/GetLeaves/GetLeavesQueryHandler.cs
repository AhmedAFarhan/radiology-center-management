using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetLeaves;

public static class GetLeavesQueryHandler
{
    public static async Task<Result<PagedResult<LeaveDto>>> HandleAsync(
        GetLeavesQuery query,
        ILeaveRepository leaveRepository,
        CancellationToken ct)
    {
        var paged = await leaveRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(l => l.Adapt<LeaveDto>()).ToList();

        return Result.Success(new PagedResult<LeaveDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
