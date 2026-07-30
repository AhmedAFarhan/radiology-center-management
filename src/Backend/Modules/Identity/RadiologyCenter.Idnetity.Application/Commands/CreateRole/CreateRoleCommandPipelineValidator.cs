using FluentValidation;
using RadiologyCenter.Idnetity.Application.Abstractions;

namespace RadiologyCenter.Idnetity.Application.Commands.CreateRole;

public class CreateRoleCommandPipelineValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandPipelineValidator(IRoleRepository roleRepository)
    {
        RuleFor(x => x.Name).MustAsync(async (name, ct) =>
            !await roleRepository.ExistsByNameAsync(name, ct))
            .WithMessage("Role name already exists.");
    }
}
