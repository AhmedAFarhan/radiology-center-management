using FluentValidation;
using RadiologyCenter.Cash.Application.Commands.Sessions.Common;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.AddCashEntry;

public class AddCashEntryCommandValidator : AbstractValidator<AddCashEntryCommand>
{
    public AddCashEntryCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty();
        RuleFor(x => x.Direction).IsInEnum();
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ReferenceId).MaximumLength(100);
    }
}