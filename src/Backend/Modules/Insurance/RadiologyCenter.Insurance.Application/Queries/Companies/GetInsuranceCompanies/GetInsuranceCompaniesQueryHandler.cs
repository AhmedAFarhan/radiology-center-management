using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanies;

public static class GetInsuranceCompaniesQueryHandler
{
    public static async Task<Result<IReadOnlyList<InsuranceCompanyDto>>> HandleAsync(
        IInsuranceCompanyRepository companyRepository,
        CancellationToken ct)
    {
        var companies = await companyRepository.GetAllAsync(ct);
        return Result.Success<IReadOnlyList<InsuranceCompanyDto>>(
            companies.Select(c => c.ToDto()).ToList());
    }
}