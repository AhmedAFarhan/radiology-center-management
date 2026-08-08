using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.CreateInsurancePolicy;

public static class CreateInsurancePolicyCommandHandler
{
    public static async Task<Result<InsurancePolicyDto>> HandleAsync(
        CreateInsurancePolicyCommand command,
        IInsuranceCompanyRepository companyRepository,
        IInsurancePolicyRepository policyRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (await companyRepository.GetByIdAsync(command.CompanyId, ct) is null)
            return Result.Failure<InsurancePolicyDto>(Error.NotFound("Company", command.CompanyId));

        if (await policyRepository.ExistsByPolicyNumberAsync(command.PolicyNumber, ct))
            return Result.Failure<InsurancePolicyDto>(Error.Conflict($"A policy with number '{command.PolicyNumber}' already exists."));

        var policy = InsurancePolicy.Create(
            command.CompanyId,
            command.PatientId,
            command.PolicyNumber,
            command.CoveragePercent,
            command.EffectiveFrom,
            command.EffectiveTo,
            command.IsGovernment);

        await policyRepository.AddAsync(policy, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(policy.ToDto());
    }
}