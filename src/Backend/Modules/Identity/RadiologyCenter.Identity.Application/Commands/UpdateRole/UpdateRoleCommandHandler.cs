using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.UpdateRole;

public static class UpdateRoleCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateRoleCommand command,
        IRoleRepository roleRepository,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var role = await roleRepository.GetByIdAsync(command.RoleId, ct);
        if (role is null)
            return Result.Failure(Error.NotFound(ErrorCodes.RoleNotFound, "Role", command.RoleId));

        role.Update(command.Name, command.Description);
        await roleRepository.UpdateAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
