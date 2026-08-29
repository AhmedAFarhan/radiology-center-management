using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.CreateInsurancePolicy;

public class CreateInsurancePolicyCommandValidator : AbstractValidator<CreateInsurancePolicyCommand>
{
    public CreateInsurancePolicyCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithErrorCode(ErrorCodes.CompanyIdRequired);
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(ErrorCodes.PatientIdRequired);
        RuleFor(x => x.PolicyNumber).NotEmpty().WithErrorCode(ErrorCodes.PolicyNumberRequired).MaximumLength(100).WithErrorCode(ErrorCodes.PolicyNumberTooLong);
        RuleFor(x => x.CoveragePercent).InclusiveBetween(0, 100).WithErrorCode(ErrorCodes.CoveragePercentMustBeBetween);
        RuleFor(x => x.EffectiveFrom).NotEmpty().WithErrorCode(ErrorCodes.EffectiveFromRequired);
    }
}
