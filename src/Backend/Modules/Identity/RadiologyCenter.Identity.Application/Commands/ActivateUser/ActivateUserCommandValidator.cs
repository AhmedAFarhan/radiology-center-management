using FluentValidation;

namespace RadiologyCenter.Identity.Application.Commands.ActivateUser;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator() => RuleFor(x => x.UserId).NotEmpty();
}
