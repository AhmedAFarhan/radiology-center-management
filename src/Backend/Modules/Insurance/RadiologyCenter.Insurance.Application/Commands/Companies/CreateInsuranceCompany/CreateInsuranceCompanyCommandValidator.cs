using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.CreateInsuranceCompany;

public class CreateInsuranceCompanyCommandValidator : AbstractValidator<CreateInsuranceCompanyCommand>
{
    public CreateInsuranceCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.CompanyNameRequired).MaximumLength(200).WithErrorCode(ErrorCodes.CompanyNameTooLong);
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(ErrorCodes.CompanyEmailInvalid).When(x => !string.IsNullOrEmpty(x.Email));
    }
}
