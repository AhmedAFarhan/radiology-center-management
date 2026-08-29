using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Commands.RemovePermissionFromRole;

public class RemovePermissionFromRoleCommandValidator : AbstractValidator<RemovePermissionFromRoleCommand>
{
    public RemovePermissionFromRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithErrorCode(ErrorCodes.RoleIdRequired);
        RuleFor(x => x.PermissionCode).NotEmpty().WithErrorCode(ErrorCodes.PermissionCodeRequired);
    }
}
