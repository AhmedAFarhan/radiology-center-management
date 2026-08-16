using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.RemovePermissionFromRole;

public static class RemovePermissionFromRoleCommandHandler
{
    public static async Task<Result> HandleAsync(
        RemovePermissionFromRoleCommand command,
        IRoleRepository roleRepository,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var role = await roleRepository.GetByIdAsync(command.RoleId, ct);
        if (role is null)
            return Result.Failure(Error.NotFound(ErrorCodes.RoleNotFound, "Role", command.RoleId));

        var permission = Permissions.GetByCode(command.PermissionCode);
        if (permission is null)
            return Result.Failure(Error.NotFound(ErrorCodes.PermissionNotFound, "Permission", command.PermissionCode));

        role.RemovePermission(permission);
        await roleRepository.UpdateAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
