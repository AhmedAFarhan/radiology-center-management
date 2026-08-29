using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator() => RuleFor(x => x.Token).NotEmpty().WithErrorCode(ErrorCodes.TokenRequired);
}
