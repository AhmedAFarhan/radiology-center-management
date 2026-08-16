using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.LockUser;

public static class LockUserCommandHandler
{
    public static async Task<Result> HandleAsync(
        LockUserCommand command,
        IUserRepository userRepository,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound(ErrorCodes.UserNotFound, "User", command.UserId));

        user.Lock(command.LockoutEnd);
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
