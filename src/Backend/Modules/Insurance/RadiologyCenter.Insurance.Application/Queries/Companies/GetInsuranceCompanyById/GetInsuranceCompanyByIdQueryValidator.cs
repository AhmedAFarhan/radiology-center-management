using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanyById;

public class GetInsuranceCompanyByIdQueryValidator : AbstractValidator<GetInsuranceCompanyByIdQuery>
{
    public GetInsuranceCompanyByIdQueryValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithErrorCode(ErrorCodes.CompanyIdRequired);
    }
}
