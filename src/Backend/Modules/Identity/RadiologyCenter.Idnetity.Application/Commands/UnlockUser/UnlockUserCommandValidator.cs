using FluentValidation;

namespace RadiologyCenter.Idnetity.Application.Commands.UnlockUser;

public class UnlockUserCommandValidator : AbstractValidator<UnlockUserCommand>
{
    public UnlockUserCommandValidator() => RuleFor(x => x.UserId).NotEmpty();
}
