using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.CreateUser;

public static class CreateUserCommandHandler
{
    public static async Task<Result> HandleAsync(
        CreateUserCommand command,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IIdentityUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        ICurrentUser currentUser,
        IIdentityAccessProvisioner accessProvisioner,
        CancellationToken ct)
    {
        var user = User.Create(command.UserName, command.Email, command.FirstName, command.LastName, command.PhoneNumber);
        user.SetPasswordHash(passwordHasher.HashPassword(user, command.Password));
        user.RequirePasswordChange();

        var roles = await roleRepository.GetByIdsAsync(command.RoleIds, ct);
        var missingRoleId = command.RoleIds.Distinct().FirstOrDefault(id => roles.All(r => r.Id != id));
        if (missingRoleId != Guid.Empty)
            return Result.Failure(Error.NotFound(ErrorCodes.RoleNotFound, "Role", missingRoleId));

        if (!currentUser.HasPermission(RadiologyCenter.Identity.Domain.Permissions.UsersManageRolesCode)
            || (roles.Any(r => r.IsSystem) && !currentUser.IsAdministrator))
            return Result.Failure(Error.Forbidden("Identity.RoleAssignmentForbidden", "You cannot assign the requested roles."));
        user.UpdateRoles(roles);

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await accessProvisioner.ProvisionAsync(user.Id,command.Password,ct);
        await transaction.CommitAsync(ct);
        return Result.Success();
    }
}
