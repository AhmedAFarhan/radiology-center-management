using FluentValidation;

namespace RadiologyCenter.Idnetity.Application.Commands.ActivateUser;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator() => RuleFor(x => x.UserId).NotEmpty();
}
