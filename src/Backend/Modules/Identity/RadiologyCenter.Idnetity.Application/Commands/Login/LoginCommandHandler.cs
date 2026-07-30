using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.Login;

public static class LoginCommandHandler
{
    public static async Task<Result> HandleAsync(
        LoginCommand command,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByUserNameAsync(command.UserName, ct);
        if (user is null || !user.IsActive)
            return Result.Failure(Error.Unauthorized("Invalid username or password."));

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, command.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            user.IncrementAccessFailedCount();
            await userRepository.UpdateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Failure(Error.Unauthorized("Invalid username or password."));
        }

        user.RecordLogin();
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
