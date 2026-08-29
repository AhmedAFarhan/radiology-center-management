using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Commands.AddPermissionToRole;

public class AddPermissionToRoleCommandValidator : AbstractValidator<AddPermissionToRoleCommand>
{
    public AddPermissionToRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithErrorCode(ErrorCodes.RoleIdRequired);
        RuleFor(x => x.PermissionCode).NotEmpty().WithErrorCode(ErrorCodes.PermissionCodeRequired);
    }
}
