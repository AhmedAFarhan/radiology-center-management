using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.DecidePreAuthorization;

public class DecidePreAuthorizationCommandValidator : AbstractValidator<DecidePreAuthorizationCommand>
{
    public DecidePreAuthorizationCommandValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty();
        RuleFor(x => x.Decision).IsInEnum();
        RuleFor(x => x.ApprovedAmount).GreaterThanOrEqualTo(0).When(x => x.ApprovedAmount.HasValue);
        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .When(x => x.Decision == PreAuthorizationDecision.Deny);
    }
}