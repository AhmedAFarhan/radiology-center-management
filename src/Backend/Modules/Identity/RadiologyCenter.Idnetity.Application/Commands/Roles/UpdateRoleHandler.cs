using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.Roles;

public record UpdateRoleCommand(Guid RoleId, string Name, string? Description);

public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public static class UpdateRoleHandler
{
    public static async Task<Result> HandleAsync(
        UpdateRoleCommand command,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var role = await roleRepository.GetByIdAsync(command.RoleId, ct);
        if (role is null)
            return Result.Failure(Error.NotFound("Role", command.RoleId));

        var roleName = role!.Name;
        if (!roleName!.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
        {
            var nameExists = await roleRepository.ExistsByNameAsync(command.Name, ct);
            if (nameExists)
                return Result.Failure(Error.Conflict($"Role '{command.Name}' already exists."));
        }

        role.Update(command.Name, command.Description);
        await roleRepository.UpdateAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
