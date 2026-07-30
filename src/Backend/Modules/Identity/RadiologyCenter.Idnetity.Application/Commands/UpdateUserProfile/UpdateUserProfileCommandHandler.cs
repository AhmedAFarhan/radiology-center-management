using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.UpdateUserProfile;

public static class UpdateUserProfileCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateUserProfileCommand command,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound("User", command.UserId));

        user.UpdateProfile(command.FirstName, command.LastName, command.PhoneNumber);
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
