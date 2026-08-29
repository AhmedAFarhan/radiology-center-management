using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.UpdateInsuranceCompany;

public class UpdateInsuranceCompanyCommandValidator : AbstractValidator<UpdateInsuranceCompanyCommand>
{
    public UpdateInsuranceCompanyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.CompanyIdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.CompanyNameRequired).MaximumLength(200).WithErrorCode(ErrorCodes.CompanyNameTooLong);
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(ErrorCodes.CompanyEmailInvalid).When(x => !string.IsNullOrEmpty(x.Email));
    }
}
