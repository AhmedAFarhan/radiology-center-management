using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.LockUser;

public static class LockUserCommandHandler
{
    public static async Task<Result> HandleAsync(
        LockUserCommand command,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound("User", command.UserId));

        user.Lock(command.LockoutEnd);
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
