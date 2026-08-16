using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.ActivateUser;

public static class ActivateUserCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateUserCommand command,
        IUserRepository userRepository,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound(ErrorCodes.UserNotFound, "User", command.UserId));

        user.Activate();
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
