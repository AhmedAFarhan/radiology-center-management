using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator() => RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.RoleNameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.RoleNameTooLong);
}
