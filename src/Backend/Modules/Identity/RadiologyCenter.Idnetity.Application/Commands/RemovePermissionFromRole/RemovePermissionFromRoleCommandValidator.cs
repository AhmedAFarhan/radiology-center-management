using FluentValidation;

namespace RadiologyCenter.Idnetity.Application.Commands.RemovePermissionFromRole;

public class RemovePermissionFromRoleCommandValidator : AbstractValidator<RemovePermissionFromRoleCommand>
{
    public RemovePermissionFromRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.PermissionCode).NotEmpty();
    }
}
