using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.Logout;

public static class LogoutCommandHandler
{
    public static async Task<Result> HandleAsync(
        LogoutCommand command,
        IUserRepository userRepository,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound("User", command.UserId));

        if (command.RefreshToken is not null)
        {
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
