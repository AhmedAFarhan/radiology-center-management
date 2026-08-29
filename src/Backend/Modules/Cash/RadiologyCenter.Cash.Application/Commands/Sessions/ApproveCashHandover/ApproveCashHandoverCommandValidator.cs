using FluentValidation;
using RadiologyCenter.Cash.Application.Localization;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.ApproveCashHandover;

public class ApproveCashHandoverCommandValidator : AbstractValidator<ApproveCashHandoverCommand>
{
    public ApproveCashHandoverCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty().WithErrorCode(ErrorCodes.SessionIdRequired);
    }
}