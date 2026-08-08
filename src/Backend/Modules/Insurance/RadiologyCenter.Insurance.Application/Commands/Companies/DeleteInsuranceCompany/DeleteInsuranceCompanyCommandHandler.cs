using RadiologyCenter.Insurance.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Commands.Companies.DeleteInsuranceCompany;

public static class DeleteInsuranceCompanyCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteInsuranceCompanyCommand command,
        IInsuranceCompanyRepository companyRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var company = await companyRepository.GetByIdAsync(command.Id, ct);
        if (company is null)
            return Result.Failure(Error.NotFound("Company", command.Id));

        company.Delete(by: null);

        companyRepository.Update(company);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}