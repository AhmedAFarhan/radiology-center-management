using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Cash.Application.Commands.Sessions.Common;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.AddCashEntry;

public class AddCashEntryCommandValidator : AbstractValidator<AddCashEntryCommand>
{
    public AddCashEntryCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty().WithErrorCode(ErrorCodes.Shared.IdRequired);
        RuleFor(x => x.Direction).IsInEnum().WithErrorCode(ErrorCodes.Shared.InvalidEnumValue);
        RuleFor(x => x.Reason).IsInEnum().WithErrorCode(ErrorCodes.Shared.InvalidEnumValue);
        RuleFor(x => x.Amount).GreaterThan(0).WithErrorCode(ErrorCodes.Shared.ValueMustBePositive);
        RuleFor(x => x.Description).MaximumLength(500).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.ReferenceId).MaximumLength(100).WithErrorCode(ErrorCodes.Shared.TextTooLong);
    }
}