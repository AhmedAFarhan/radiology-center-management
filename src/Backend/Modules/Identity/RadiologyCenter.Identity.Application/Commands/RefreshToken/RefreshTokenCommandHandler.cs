using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.DTOs;

namespace RadiologyCenter.Identity.Application.Commands.RefreshToken;

public static class RefreshTokenCommandHandler
{
    public static async Task<Result<TokenResult>> HandleAsync(
        RefreshTokenCommand command,
        IUserRepository userRepository,
        ITokenService tokenService,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByRefreshTokenAsync(command.Token, ct);
        if (user is null)
            return Result.Failure<TokenResult>(Error.Unauthorized(ErrorCodes.InvalidRefreshToken, "Invalid refresh token."));

        if (!user.IsActive)
            return Result.Failure<TokenResult>(Error.Unauthorized(ErrorCodes.AccountDeactivated, "Account is deactivated."));

        if (user.IsLockedOut)
            return Result.Failure<TokenResult>(Error.LockedOut(ErrorCodes.AccountLockedOut, "Account is locked due to too many failed login attempts."));

        if (!user.HasValidRefreshToken(command.Token) || !user.HasActiveSession(command.Token))
            return Result.Failure<TokenResult>(Error.Unauthorized(ErrorCodes.RefreshTokenExpired, "Refresh token is expired or revoked."));

        user.RevokeRefreshToken(command.Token);
        user.RevokeSession(command.Token);

        var tokenResult = tokenService.GenerateTokenResult(user);

        user.AddRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt);
        user.StartSession(tokenResult.RefreshToken);
        user.RecordSessionActivity(command.Token);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(tokenResult);
    }
}
