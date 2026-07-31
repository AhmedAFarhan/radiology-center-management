using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.DTOs;

namespace RadiologyCenter.Identity.Application.Commands.RefreshToken;

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

        var tokenResult = tokenService.GenerateTokenResult(user);

        user.AddRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(tokenResult);
    }
}
