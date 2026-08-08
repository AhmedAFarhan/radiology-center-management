using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.CreateInsuranceCompany;

public static class CreateInsuranceCompanyCommandHandler
{
    public static async Task<Result<InsuranceCompanyDto>> HandleAsync(
        CreateInsuranceCompanyCommand command,
        IInsuranceCompanyRepository companyRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (await companyRepository.GetByNameAsync(command.Name, ct) is not null)
            return Result.Failure<InsuranceCompanyDto>(Error.Conflict($"An insurance company named '{command.Name}' already exists."));

        var company = InsuranceCompany.Create(
            command.Name,
            command.TaxId,
            command.Address,
            command.Phone,
            command.Email);

        await companyRepository.AddAsync(company, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(company.ToDto());
    }
}