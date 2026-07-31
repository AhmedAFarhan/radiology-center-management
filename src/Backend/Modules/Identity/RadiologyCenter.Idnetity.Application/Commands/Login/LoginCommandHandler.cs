using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.DTOs;

namespace RadiologyCenter.Idnetity.Application.Commands.Login;

public static class LoginCommandHandler
{
    public static async Task<Result<TokenResult>> HandleAsync(
        LoginCommand command,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByUserNameAsync(command.UserName, ct);
        if (user is null || !user.IsActive)
            return Result.Failure<TokenResult>(Error.Unauthorized("Invalid username or password."));

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, command.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            user.IncrementAccessFailedCount();
            await userRepository.UpdateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Failure<TokenResult>(Error.Unauthorized("Invalid username or password."));
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        user.RecordLogin();
        user.AddRefreshToken(refreshToken, expiresAt.AddDays(7));
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new TokenResult(accessToken, refreshToken, expiresAt));
    }
}
