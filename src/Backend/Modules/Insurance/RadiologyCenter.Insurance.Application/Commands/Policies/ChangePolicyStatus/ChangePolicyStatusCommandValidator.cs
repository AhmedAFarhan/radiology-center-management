using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.ChangePolicyStatus;

public class ChangePolicyStatusCommandValidator : AbstractValidator<ChangePolicyStatusCommand>
{
    public ChangePolicyStatusCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(ErrorCodes.PolicyIdRequired);
        RuleFor(x => x.Action).IsInEnum().WithErrorCode(ErrorCodes.PolicyActionInvalid);
    }
}
