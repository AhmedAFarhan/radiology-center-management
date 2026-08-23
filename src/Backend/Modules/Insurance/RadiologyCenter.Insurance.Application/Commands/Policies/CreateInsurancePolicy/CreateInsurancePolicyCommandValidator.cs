using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.CreateInsurancePolicy;

public class CreateInsurancePolicyCommandValidator : AbstractValidator<CreateInsurancePolicyCommand>
{
    public CreateInsurancePolicyCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.PolicyNumber).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.CoveragePercent).InclusiveBetween(0, 100).WithErrorCode(SharedCodes.Shared.MustBeBetween);
        RuleFor(x => x.EffectiveFrom).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
    }
}