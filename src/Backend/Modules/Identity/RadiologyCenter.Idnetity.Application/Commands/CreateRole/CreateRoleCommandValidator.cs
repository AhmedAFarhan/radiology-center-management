using FluentValidation;

namespace RadiologyCenter.Idnetity.Application.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
}
