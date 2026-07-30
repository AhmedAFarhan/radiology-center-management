using FluentValidation;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.DTOs;

namespace RadiologyCenter.Idnetity.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id);

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator() => RuleFor(x => x.Id).NotEmpty();
}

public static class GetUserByIdHandler
{
    public static async Task<Result<UserDto>> HandleAsync(
        GetUserByIdQuery query,
        IUserRepository userRepository,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(query.Id, ct);
        if (user is null)
            return Result.Failure<UserDto>(Error.NotFound("User", query.Id));

        return Result.Success(Map(user));
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
