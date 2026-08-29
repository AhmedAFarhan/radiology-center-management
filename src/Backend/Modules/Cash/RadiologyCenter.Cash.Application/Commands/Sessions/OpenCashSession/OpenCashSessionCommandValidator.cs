using FluentValidation;
using RadiologyCenter.Cash.Application.Localization;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.OpenCashSession;

public class OpenCashSessionCommandValidator : AbstractValidator<OpenCashSessionCommand>
{
    public OpenCashSessionCommandValidator()
    {
        RuleFor(x => x.OpeningFloat).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.OpeningFloatCannotBeNegative);
        RuleFor(x => x.Notes).MaximumLength(1000).WithErrorCode(ErrorCodes.NotesTooLong);
    }
}