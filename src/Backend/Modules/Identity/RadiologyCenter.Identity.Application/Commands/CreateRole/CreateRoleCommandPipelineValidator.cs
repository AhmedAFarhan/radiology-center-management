using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.Identity.Application.Abstractions;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.CreateRole;

public class CreateRoleCommandPipelineValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandPipelineValidator(IRoleRepository roleRepository)
    {
        RuleFor(x => x.Name).MustAsync(async (name, ct) =>
            !await roleRepository.ExistsByNameAsync(name, ct))
            .WithErrorCode(ErrorCodes.RoleNameExists);
    }
}
