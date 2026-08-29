using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(ErrorCodes.UserIdRequired);
        RuleFor(x => x.RoleIds).NotNull().WithErrorCode(ErrorCodes.AtLeastOneRole).Must(ids => ids.Count > 0).WithErrorCode(ErrorCodes.AtLeastOneRole);
        RuleFor(x => x.RoleIds).Must(ids => ids.All(id => id != Guid.Empty)).WithErrorCode(ErrorCodes.RoleIdsNotEmpty);
    }
}
