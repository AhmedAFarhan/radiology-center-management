using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Localization;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.ApproveCashHandover;

public class ApproveCashHandoverCommandValidator : AbstractValidator<ApproveCashHandoverCommand>
{
    public ApproveCashHandoverCommandValidator()
    {
        RuleFor(x => x.CashSessionId).NotEmpty().WithErrorCode(ErrorCodes.Shared.IdRequired);
    }
}