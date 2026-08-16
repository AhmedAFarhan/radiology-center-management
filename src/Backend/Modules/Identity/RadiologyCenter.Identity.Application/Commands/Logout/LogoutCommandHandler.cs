using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.Logout;

public static class LogoutCommandHandler
{
    public static async Task<Result> HandleAsync(
        LogoutCommand command,
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var userId))
            return Result.Failure(Error.Unauthorized());

        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure(Error.Unauthorized());

        if (command.RefreshToken is not null)
        {
            if (!user.HasValidRefreshToken(command.RefreshToken) || !user.HasActiveSession(command.RefreshToken))
                return Result.Failure(Error.Unauthorized(ErrorCodes.RefreshTokenExpired, "Refresh token is expired or revoked."));

            user.RevokeRefreshToken(command.RefreshToken);
            user.RevokeSession(command.RefreshToken);
        }
        else
        {
            user.RevokeAllRefreshTokens();
            user.RevokeAllSessions();
        }

        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
