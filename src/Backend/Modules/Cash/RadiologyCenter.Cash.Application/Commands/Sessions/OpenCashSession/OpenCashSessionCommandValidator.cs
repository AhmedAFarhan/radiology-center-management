using FluentValidation;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.OpenCashSession;

public class OpenCashSessionCommandValidator : AbstractValidator<OpenCashSessionCommand>
{
    public OpenCashSessionCommandValidator()
    {
        RuleFor(x => x.OpeningFloat).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}