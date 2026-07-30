using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.CreateRole;

public static class CreateRoleCommandHandler
{
    public static async Task<Result> HandleAsync(
        CreateRoleCommand command,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var role = Role.Create(command.Name, command.Description, command.IsSystem);
        await roleRepository.AddAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
