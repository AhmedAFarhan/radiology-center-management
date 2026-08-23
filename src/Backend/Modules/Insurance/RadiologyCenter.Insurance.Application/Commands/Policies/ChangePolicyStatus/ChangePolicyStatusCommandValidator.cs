using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.ChangePolicyStatus;

public class ChangePolicyStatusCommandValidator : AbstractValidator<ChangePolicyStatusCommand>
{
    public ChangePolicyStatusCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Action).IsInEnum().WithErrorCode(SharedCodes.Shared.InvalidEnumValue);
    }
}