using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.CreateRole;

public static class CreateRoleCommandHandler
{
    public static async Task<Result> HandleAsync(
        CreateRoleCommand command,
        IRoleRepository roleRepository,
        IIdentityUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        CancellationToken ct)
    {
        if (command.IsSystem || string.Equals(command.Name, "Administrator", StringComparison.OrdinalIgnoreCase))
            return Result.Failure(Error.Forbidden("Identity.ReservedRole", "System roles can only be provisioned by the server."));
        var role = Role.Create(command.Name, command.Description);
        await roleRepository.AddAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
