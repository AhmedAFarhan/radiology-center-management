using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.AddPermissionToRole;

public static class AddPermissionToRoleCommandHandler
{
    public static async Task<Result> HandleAsync(
        AddPermissionToRoleCommand command,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var role = await roleRepository.GetByIdAsync(command.RoleId, ct);
        if (role is null)
            return Result.Failure(Error.NotFound("Role", command.RoleId));

        var permission = Permissions.GetByCode(command.PermissionCode);
        if (permission is null)
            return Result.Failure(Error.NotFound("Permission", command.PermissionCode));

        role.AddPermission(permission);
        await roleRepository.UpdateAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
