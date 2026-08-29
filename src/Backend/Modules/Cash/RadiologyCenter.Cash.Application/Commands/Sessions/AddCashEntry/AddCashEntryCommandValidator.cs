using FluentValidation;
using RadiologyCenter.Cash.Application.Commands.Sessions.Common;
using RadiologyCenter.Cash.Application.Localization;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.AddCashEntry;

public class AddCashEntryCommandValidator : AbstractValidator<AddCashEntryCommand>
{
    public AddCashEntryCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty().WithErrorCode(ErrorCodes.SessionIdRequired);
        RuleFor(x => x.Direction).IsInEnum().WithErrorCode(ErrorCodes.DirectionInvalid);
        RuleFor(x => x.Reason).IsInEnum().WithErrorCode(ErrorCodes.ReasonInvalid);
        RuleFor(x => x.Amount).GreaterThan(0).WithErrorCode(ErrorCodes.AmountMustBePositive);
        RuleFor(x => x.Description).MaximumLength(500).WithErrorCode(ErrorCodes.DescriptionTooLong);
        RuleFor(x => x.ReferenceId).MaximumLength(100).WithErrorCode(ErrorCodes.ReferenceIdTooLong);
    }
}