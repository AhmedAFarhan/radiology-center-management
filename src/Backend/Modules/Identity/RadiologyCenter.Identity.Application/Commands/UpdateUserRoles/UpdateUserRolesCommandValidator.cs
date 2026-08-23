using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.RoleIds).NotNull().WithErrorCode(SharedCodes.Shared.FieldRequired).Must(ids => ids.Count > 0).WithErrorCode(ErrorCodes.AtLeastOneRole);
        RuleFor(x => x.RoleIds).Must(ids => ids.All(id => id != Guid.Empty)).WithErrorCode(ErrorCodes.RoleIdsNotEmpty);
    }
}
