using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.UpdateInsuranceCompany;

public class UpdateInsuranceCompanyCommandValidator : AbstractValidator<UpdateInsuranceCompanyCommand>
{
    public UpdateInsuranceCompanyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}