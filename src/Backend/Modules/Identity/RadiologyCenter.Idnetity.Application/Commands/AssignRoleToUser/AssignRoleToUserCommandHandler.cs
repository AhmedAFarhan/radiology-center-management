using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.AssignRoleToUser;

public static class AssignRoleToUserCommandHandler
{
    public static async Task<Result> HandleAsync(
        AssignRoleToUserCommand command,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound("User", command.UserId));

        var role = await roleRepository.GetByIdAsync(command.RoleId, ct);
        if (role is null)
            return Result.Failure(Error.NotFound("Role", command.RoleId));

        user.AssignRole(role);
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
