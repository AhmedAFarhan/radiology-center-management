using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.CreateInsuranceCompany;

public class CreateInsuranceCompanyCommandValidator : AbstractValidator<CreateInsuranceCompanyCommand>
{
    public CreateInsuranceCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(SharedCodes.Shared.InvalidEmail).When(x => !string.IsNullOrEmpty(x.Email));
    }
}