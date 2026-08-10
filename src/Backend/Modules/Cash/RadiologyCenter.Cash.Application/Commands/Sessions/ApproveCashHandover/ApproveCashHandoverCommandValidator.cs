using FluentValidation;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.ApproveCashHandover;

public class ApproveCashHandoverCommandValidator : AbstractValidator<ApproveCashHandoverCommand>
{
    public ApproveCashHandoverCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty();
    }
}