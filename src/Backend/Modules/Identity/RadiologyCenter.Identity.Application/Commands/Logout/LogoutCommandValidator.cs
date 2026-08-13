using FluentValidation;

namespace RadiologyCenter.Identity.Application.Commands.Logout;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .When(x => x.RefreshToken is not null);
    }
}
