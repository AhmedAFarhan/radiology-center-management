using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.UpdateInsuranceCompany;

public static class UpdateInsuranceCompanyCommandHandler
{
    public static async Task<Result<InsuranceCompanyDto>> HandleAsync(
        UpdateInsuranceCompanyCommand command,
        IInsuranceCompanyRepository companyRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var company = await companyRepository.GetByIdAsync(command.Id, ct);
        if (company is null)
            return Result.Failure<InsuranceCompanyDto>(Error.NotFound(ErrorCodes.CompanyNotFound, "InsuranceCompany", command.Id));

        var nameTaken = await companyRepository.GetByNameAsync(command.Name, ct);
        if (nameTaken is not null && nameTaken.Id != command.Id)
            return Result.Failure<InsuranceCompanyDto>(Error.Conflict(ErrorCodes.CompanyNameExists, $"An insurance company named '{command.Name}' already exists."));

        company.Update(command.Name, command.TaxId, command.Address, command.Phone, command.Email);

        companyRepository.Update(company);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(company.ToDto());
    }
}