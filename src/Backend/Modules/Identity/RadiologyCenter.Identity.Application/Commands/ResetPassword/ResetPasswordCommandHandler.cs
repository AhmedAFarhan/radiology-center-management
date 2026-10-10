using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.ResetPassword;

public static class ResetPasswordCommandHandler
{
    public static async Task<Result> HandleAsync(
        ResetPasswordCommand command,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IIdentityUnitOfWork unitOfWork,
        IIdentityAccessProvisioner accessProvisioner,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound(ErrorCodes.UserNotFound, "User", command.UserId));

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        user.SetPasswordHash(passwordHasher.HashPassword(user, command.NewPassword));
        user.RequirePasswordChange();
        user.RevokeAllRefreshTokens();
        user.RevokeAllSessions();
        user.Unlock();

        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await accessProvisioner.ProvisionAsync(user.Id,command.NewPassword,ct);
        await transaction.CommitAsync(ct);
        return Result.Success();
    }
}
