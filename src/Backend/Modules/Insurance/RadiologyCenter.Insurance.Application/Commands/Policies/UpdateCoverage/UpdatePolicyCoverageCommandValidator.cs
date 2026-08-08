using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.UpdateCoverage;

public class UpdatePolicyCoverageCommandValidator : AbstractValidator<UpdatePolicyCoverageCommand>
{
    public UpdatePolicyCoverageCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.CoveragePercent).InclusiveBetween(0, 100);
        RuleFor(x => x.Deductible).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Copay).GreaterThanOrEqualTo(0);
    }
}