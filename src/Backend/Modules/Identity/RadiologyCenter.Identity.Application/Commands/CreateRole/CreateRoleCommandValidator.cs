using FluentValidation;

namespace RadiologyCenter.Identity.Application.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
}
