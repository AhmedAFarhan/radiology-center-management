using Mapster;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.DTOs;

namespace RadiologyCenter.Identity.Application.Queries.GetUserById;

public static class GetUserByIdQueryHandler
{
    public static async Task<Result<UserDto>> HandleAsync(
        GetUserByIdQuery query,
        IUserRepository userRepository,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(query.Id, ct);
        if (user is null)
            return Result.Failure<UserDto>(Error.NotFound("User", query.Id));

        return Result.Success(user.Adapt<UserDto>());
    }
}
