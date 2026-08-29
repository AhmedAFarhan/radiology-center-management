using FluentValidation;
using RadiologyCenter.Cash.Application.Localization;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.CloseCashSession;

public class CloseCashSessionCommandValidator : AbstractValidator<CloseCashSessionCommand>
{
    public CloseCashSessionCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty().WithErrorCode(ErrorCodes.SessionIdRequired);
        RuleFor(x => x.CountedTotal).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.CountedTotalCannotBeNegative);
        RuleFor(x => x.ReceivingOpeningFloat).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ReceivingOpeningFloatCannotBeNegative).When(x => x.ReceivingOpeningFloat.HasValue);
        RuleFor(x => x.Notes).MaximumLength(1000).WithErrorCode(ErrorCodes.NotesTooLong);
    }
}