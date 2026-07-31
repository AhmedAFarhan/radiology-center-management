using FluentValidation;

namespace RadiologyCenter.Identity.Application.Commands.UnlockUser;

public class UnlockUserCommandValidator : AbstractValidator<UnlockUserCommand>
{
    public UnlockUserCommandValidator() => RuleFor(x => x.UserId).NotEmpty();
}
