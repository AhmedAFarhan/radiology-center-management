using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.DTOs;

namespace RadiologyCenter.Identity.Application.Commands.ChangePassword;

public static class ChangePasswordCommandHandler
{
    public static async Task<Result<TokenResult>> HandleAsync(
        ChangePasswordCommand command,
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var userId))
            return Result.Failure<TokenResult>(Error.Unauthorized());

        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure<TokenResult>(Error.Unauthorized());

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, command.CurrentPassword);
        if (verification == PasswordVerificationResult.Failed)
            return Result.Failure<TokenResult>(Error.Conflict(ErrorCodes.CurrentPasswordIncorrect, "Current password is incorrect."));

        user.SetPasswordHash(passwordHasher.HashPassword(user, command.NewPassword));
        user.PasswordChanged();
        user.RevokeAllRefreshTokens();
        user.RevokeAllSessions();

        var tokenResult = tokenService.GenerateTokenResult(user);
        user.AddRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt);
        user.StartSession(tokenResult.RefreshToken);

        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(tokenResult);
    }
}