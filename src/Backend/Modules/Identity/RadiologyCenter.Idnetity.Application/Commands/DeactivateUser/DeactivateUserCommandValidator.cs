using FluentValidation;

namespace RadiologyCenter.Idnetity.Application.Commands.DeactivateUser;

public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator() => RuleFor(x => x.UserId).NotEmpty();
}
