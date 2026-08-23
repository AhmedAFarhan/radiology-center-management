using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Localization;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.CloseCashSession;

public class CloseCashSessionCommandValidator : AbstractValidator<CloseCashSessionCommand>
{
    public CloseCashSessionCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty().WithErrorCode(ErrorCodes.Shared.IdRequired);
        RuleFor(x => x.CountedTotal).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative);
        RuleFor(x => x.ReceivingOpeningFloat).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative).When(x => x.ReceivingOpeningFloat.HasValue);
        RuleFor(x => x.Notes).MaximumLength(1000).WithErrorCode(ErrorCodes.Shared.TextTooLong);
    }
}