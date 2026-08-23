using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
    }
}
