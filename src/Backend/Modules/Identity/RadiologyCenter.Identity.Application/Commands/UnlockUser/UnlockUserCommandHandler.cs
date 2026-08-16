using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.UnlockUser;

public static class UnlockUserCommandHandler
{
    public static async Task<Result> HandleAsync(
        UnlockUserCommand command,
        IUserRepository userRepository,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound(ErrorCodes.UserNotFound, "User", command.UserId));

        user.Unlock();
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
