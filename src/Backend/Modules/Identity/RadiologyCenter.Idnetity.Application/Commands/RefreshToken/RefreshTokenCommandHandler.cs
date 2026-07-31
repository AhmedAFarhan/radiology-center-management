using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.DTOs;

namespace RadiologyCenter.Idnetity.Application.Commands.RefreshToken;

public static class RefreshTokenCommandHandler
{
    public static async Task<Result<TokenResult>> HandleAsync(
        RefreshTokenCommand command,
        IUserRepository userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByRefreshTokenAsync(command.Token, ct);
        if (user is null)
            return Result.Failure<TokenResult>(Error.Unauthorized("Invalid refresh token."));

        if (!user.HasValidRefreshToken(command.Token))
            return Result.Failure<TokenResult>(Error.Unauthorized("Refresh token is expired or revoked."));

        user.RevokeRefreshToken(command.Token);

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        user.AddRefreshToken(refreshToken, expiresAt.AddDays(7));

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new TokenResult(accessToken, refreshToken, expiresAt));
    }
}
