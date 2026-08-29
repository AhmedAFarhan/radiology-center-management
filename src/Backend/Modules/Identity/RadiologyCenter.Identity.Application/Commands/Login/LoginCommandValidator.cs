using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithErrorCode(ErrorCodes.UserNameRequired);
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.PasswordRequired);
    }
}
