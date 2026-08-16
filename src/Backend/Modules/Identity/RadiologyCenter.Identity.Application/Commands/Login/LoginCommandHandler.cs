using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Identity.Application.Settings;

namespace RadiologyCenter.Identity.Application.Commands.Login;

public static class LoginCommandHandler
{
    public static async Task<Result<TokenResult>> HandleAsync(
        LoginCommand command,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        IIdentityUnitOfWork unitOfWork,
        IOptions<AccountLockoutOptions> AccountLockoutOptions,
        CancellationToken ct)
    {
        var user = await userRepository.GetByUserNameAsync(command.UserName, ct);
        if (user is null || !user.IsActive)
            return Result.Failure<TokenResult>(Error.Conflict(ErrorCodes.InvalidCredentials, "Invalid username or password."));

        if (user.IsLockedOut)
            return Result.Failure<TokenResult>(Error.LockedOut(ErrorCodes.AccountLockedOut, "Account is locked due to too many failed login attempts."));

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, command.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            user.RegisterFailedLoginAttempt(
                AccountLockoutOptions.Value.MaxFailedAccessAttempts,
                TimeSpan.FromMinutes(AccountLockoutOptions.Value.LockoutDurationMinutes));

            await userRepository.UpdateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Failure<TokenResult>(Error.Conflict(ErrorCodes.InvalidCredentials, "Invalid username or password."));
        }

        var tokenResult = tokenService.GenerateTokenResult(user);

        user.RecordLogin();
        user.RevokeAllRefreshTokens();
        user.RevokeAllSessions();
        user.AddRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt);
        user.StartSession(tokenResult.RefreshToken);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(tokenResult);
    }
}
