using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Commands.UpdateRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithErrorCode(ErrorCodes.RoleIdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.RoleNameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.RoleNameTooLong);
    }
}
