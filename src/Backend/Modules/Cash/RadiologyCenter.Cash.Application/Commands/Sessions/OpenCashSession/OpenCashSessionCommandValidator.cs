using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Localization;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.OpenCashSession;

public class OpenCashSessionCommandValidator : AbstractValidator<OpenCashSessionCommand>
{
    public OpenCashSessionCommandValidator()
    {
        RuleFor(x => x.OpeningFloat).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Notes).MaximumLength(1000).WithErrorCode(ErrorCodes.Shared.TextTooLong);
    }
}