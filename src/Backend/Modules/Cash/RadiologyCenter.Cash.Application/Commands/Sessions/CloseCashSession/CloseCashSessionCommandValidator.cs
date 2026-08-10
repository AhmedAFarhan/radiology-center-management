using FluentValidation;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.CloseCashSession;

public class CloseCashSessionCommandValidator : AbstractValidator<CloseCashSessionCommand>
{
    public CloseCashSessionCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty();
        RuleFor(x => x.CountedTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReceivingOpeningFloat).GreaterThanOrEqualTo(0).When(x => x.ReceivingOpeningFloat.HasValue);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}