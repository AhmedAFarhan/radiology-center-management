using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.ChangePolicyStatus;

public class ChangePolicyStatusCommandValidator : AbstractValidator<ChangePolicyStatusCommand>
{
    public ChangePolicyStatusCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.Action).IsInEnum();
    }
}