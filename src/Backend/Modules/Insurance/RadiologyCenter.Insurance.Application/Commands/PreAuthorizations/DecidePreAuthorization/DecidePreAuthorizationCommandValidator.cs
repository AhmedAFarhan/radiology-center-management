using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DecidePreAuthorization;

public class DecidePreAuthorizationCommandValidator : AbstractValidator<DecidePreAuthorizationCommand>
{
    public DecidePreAuthorizationCommandValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(ErrorCodes.PreAuthorizationIdRequired);
        RuleFor(x => x.Decision).IsInEnum().WithErrorCode(ErrorCodes.DecisionInvalid);
        RuleFor(x => x.ApprovedAmount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ApprovedAmountCannotBeNegative).When(x => x.ApprovedAmount.HasValue);
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithErrorCode(ErrorCodes.RejectionReasonRequired)
            .When(x => x.Decision == PreAuthorizationDecision.Deny);
    }
}
