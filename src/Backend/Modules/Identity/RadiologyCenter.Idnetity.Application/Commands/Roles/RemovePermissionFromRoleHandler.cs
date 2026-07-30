using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.Roles;

public record RemovePermissionFromRoleCommand(Guid RoleId, string PermissionCode);

public class RemovePermissionFromRoleValidator : AbstractValidator<RemovePermissionFromRoleCommand>
{
    public RemovePermissionFromRoleValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.PermissionCode).NotEmpty();
    }
}

public static class RemovePermissionFromRoleHandler
{
    public static async Task<Result> HandleAsync(
        RemovePermissionFromRoleCommand command,
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

        role.RemovePermission(permission);
        await roleRepository.UpdateAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
