using FluentValidation;

namespace RadiologyCenter.Identity.Application.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleIds).NotNull().Must(ids => ids.Count > 0);
        RuleFor(x => x.RoleIds).Must(ids => ids.All(id => id != Guid.Empty));
    }
}
