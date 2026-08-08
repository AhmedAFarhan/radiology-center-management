using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.CreateInsuranceCompany;

public class CreateInsuranceCompanyCommandValidator : AbstractValidator<CreateInsuranceCompanyCommand>
{
    public CreateInsuranceCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}