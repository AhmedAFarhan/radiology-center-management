using Mapster;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.DTOs;

namespace RadiologyCenter.Idnetity.Application.Queries.GetUserById;

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
