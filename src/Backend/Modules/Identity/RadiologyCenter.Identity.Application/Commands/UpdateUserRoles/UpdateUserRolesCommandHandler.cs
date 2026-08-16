using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.UpdateUserRoles;

public static class UpdateUserRolesCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateUserRolesCommand command,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Failure(Error.NotFound(ErrorCodes.UserNotFound, "User", command.UserId));

        var roles = await roleRepository.GetByIdsAsync(command.RoleIds, ct);
        var missingRoleId = command.RoleIds.Distinct().FirstOrDefault(id => roles.All(r => r.Id != id));
        if (missingRoleId != Guid.Empty)
            return Result.Failure(Error.NotFound(ErrorCodes.RoleNotFound, "Role", missingRoleId));

        user.UpdateRoles(roles);
        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
