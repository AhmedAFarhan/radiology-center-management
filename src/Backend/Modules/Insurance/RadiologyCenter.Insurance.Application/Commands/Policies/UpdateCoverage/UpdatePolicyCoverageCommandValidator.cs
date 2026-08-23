using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.UpdateCoverage;

public class UpdatePolicyCoverageCommandValidator : AbstractValidator<UpdatePolicyCoverageCommand>
{
    public UpdatePolicyCoverageCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.CoveragePercent).InclusiveBetween(0, 100).WithErrorCode(SharedCodes.Shared.MustBeBetween);
    }
}