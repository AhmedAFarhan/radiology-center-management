using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetStaffs;

public static class GetStaffsQueryHandler
{
    public static async Task<Result<PagedResult<StaffDto>>> HandleAsync(
        GetStaffsQuery query,
        IStaffRepository staffRepository,
        CancellationToken ct)
    {
        var paged = await staffRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(s => s.Adapt<StaffDto>()).ToList();

        return Result.Success(new PagedResult<StaffDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
