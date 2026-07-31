using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.DTOs;

namespace RadiologyCenter.Identity.Application.Queries.GetUsers;

public static class GetUsersQueryHandler
{
    public static async Task<Result<PagedResult<UserDto>>> HandleAsync(
        GetUsersQuery query,
        IUserRepository userRepository,
        CancellationToken ct)
    {
        var paged = await userRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(u => u.Adapt<UserDto>()).ToList();

        return Result.Success(new PagedResult<UserDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
