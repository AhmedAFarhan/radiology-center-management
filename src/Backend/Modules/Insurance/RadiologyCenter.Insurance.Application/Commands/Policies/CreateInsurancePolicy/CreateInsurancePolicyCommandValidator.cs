using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.CreateInsurancePolicy;

public class CreateInsurancePolicyCommandValidator : AbstractValidator<CreateInsurancePolicyCommand>
{
    public CreateInsurancePolicyCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PolicyNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CoveragePercent).InclusiveBetween(0, 100);
        RuleFor(x => x.Deductible).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Copay).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EffectiveFrom).NotEmpty();
    }
}