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
        CancellationToken ct)
    {
        var user = User.Create(command.UserName, command.Email, command.FirstName, command.LastName, command.PhoneNumber);
        user.SetPasswordHash(passwordHasher.HashPassword(user, command.Password));
        user.RequirePasswordChange();

        var roles = await roleRepository.GetByIdsAsync(command.RoleIds, ct);
        var missingRoleId = command.RoleIds.Distinct().FirstOrDefault(id => roles.All(r => r.Id != id));
        if (missingRoleId != Guid.Empty)
            return Result.Failure(Error.NotFound(ErrorCodes.RoleNotFound, "Role", missingRoleId));

        user.UpdateRoles(roles);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
