using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.UpdateCoverage;

public static class UpdatePolicyCoverageCommandHandler
{
    public static async Task<Result<InsurancePolicyDto>> HandleAsync(
        UpdatePolicyCoverageCommand command,
        IInsurancePolicyRepository policyRepository,
        IInsuranceUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var policy = await policyRepository.GetByIdAsync(command.PolicyId, ct);
        if (policy is null)
            return Result.Failure<InsurancePolicyDto>(Error.NotFound("Policy", command.PolicyId));

        policy.UpdateCoverage(command.CoveragePercent, command.EffectiveTo);

        policyRepository.Update(policy);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(policy.ToDto());
    }
}