using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanyById;

public static class GetInsuranceCompanyByIdQueryHandler
{
    public static async Task<Result<InsuranceCompanyDto>> HandleAsync(
        GetInsuranceCompanyByIdQuery query,
        IInsuranceCompanyRepository companyRepository,
        CancellationToken ct)
    {
        var company = await companyRepository.GetByIdAsync(query.CompanyId, ct);
        return company is null
            ? Result.Failure<InsuranceCompanyDto>(Error.NotFound(ErrorCodes.CompanyNotFound, "Company", query.CompanyId))
            : Result.Success(company.ToDto());
    }
}