using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.DTOs;

namespace RadiologyCenter.Idnetity.Application.Queries.Users;

public record GetUsersQuery(QueryRequest Request);

public static class GetUsersHandler
{
    public static async Task<Result<PagedResult<UserDto>>> HandleAsync(
        GetUsersQuery query,
        IUserRepository userRepository,
        CancellationToken ct)
    {
        var paged = await userRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(Map).ToList();

        return Result.Success(new PagedResult<UserDto>(
            dtos,
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount
        ));
    }

    private static UserDto Map(User user) => new(
        user.Id,
        user.UserName!,
        user.Email!,
        user.FirstName,
        user.LastName,
        user.PhoneNumber,
        user.IsActive,
        user.EmailConfirmed,
        user.TwoFactorEnabled,
        user.LockoutEnabled,
        user.LockoutEnd,
        user.LastLoginAt,
        user.CreatedAt
    );
}
