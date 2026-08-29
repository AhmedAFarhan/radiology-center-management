using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.UpdateCoverage;

public class UpdatePolicyCoverageCommandValidator : AbstractValidator<UpdatePolicyCoverageCommand>
{
    public UpdatePolicyCoverageCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(ErrorCodes.PolicyIdRequired);
        RuleFor(x => x.CoveragePercent).InclusiveBetween(0, 100).WithErrorCode(ErrorCodes.CoveragePercentMustBeBetween);
    }
}
