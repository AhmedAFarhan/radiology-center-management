using FluentValidation;

namespace RadiologyCenter.Identity.Application.Commands.AddPermissionToRole;

public class AddPermissionToRoleCommandValidator : AbstractValidator<AddPermissionToRoleCommand>
{
    public AddPermissionToRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.PermissionCode).NotEmpty();
    }
}
