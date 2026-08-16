using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetInsurancePolicyById;

public static class GetInsurancePolicyByIdQueryHandler
{
    public static async Task<Result<InsurancePolicyDto>> HandleAsync(
        GetInsurancePolicyByIdQuery query,
        IInsurancePolicyRepository policyRepository,
        CancellationToken ct)
    {
        var policy = await policyRepository.GetByIdAsync(query.PolicyId, ct);
        return policy is null
            ? Result.Failure<InsurancePolicyDto>(Error.NotFound(ErrorCodes.PolicyNotFound, "Policy", query.PolicyId))
            : Result.Success(policy.ToDto());
    }
}