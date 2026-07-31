using FluentValidation;
using RadiologyCenter.Identity.Application.Abstractions;

namespace RadiologyCenter.Identity.Application.Commands.UpdateRole;

public class UpdateRoleCommandPipelineValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandPipelineValidator(IRoleRepository roleRepository)
    {
        RuleFor(x => x.Name).MustAsync(async (cmd, name, ct) =>
        {
            var existing = await roleRepository.GetByIdAsync(cmd.RoleId, ct);
            if (existing is null) return true;
            if (existing.Name!.Equals(name, StringComparison.OrdinalIgnoreCase)) return true;
            return !await roleRepository.ExistsByNameAsync(name, ct);
        }).WithMessage("Role name already exists.");
    }
}
