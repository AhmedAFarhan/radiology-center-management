using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.DeleteInsuranceCompany;

public class DeleteInsuranceCompanyCommandValidator : AbstractValidator<DeleteInsuranceCompanyCommand>
{
    public DeleteInsuranceCompanyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}