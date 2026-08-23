using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.DeleteInsuranceCompany;

public class DeleteInsuranceCompanyCommandValidator : AbstractValidator<DeleteInsuranceCompanyCommand>
{
    public DeleteInsuranceCompanyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
    }
}