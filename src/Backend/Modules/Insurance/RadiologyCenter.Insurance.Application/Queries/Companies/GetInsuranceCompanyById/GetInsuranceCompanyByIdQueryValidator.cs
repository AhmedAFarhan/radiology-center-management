using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanyById;

public class GetInsuranceCompanyByIdQueryValidator : AbstractValidator<GetInsuranceCompanyByIdQuery>
{
    public GetInsuranceCompanyByIdQueryValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}