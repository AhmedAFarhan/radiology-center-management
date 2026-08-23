using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.UpdateInsuranceCompany;

public class UpdateInsuranceCompanyCommandValidator : AbstractValidator<UpdateInsuranceCompanyCommand>
{
    public UpdateInsuranceCompanyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(SharedCodes.Shared.InvalidEmail).When(x => !string.IsNullOrEmpty(x.Email));
    }
}