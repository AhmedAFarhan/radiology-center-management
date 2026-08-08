using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPoliciesByPatient;

public static class GetPoliciesByPatientQueryHandler
{
    public static async Task<Result<IReadOnlyList<InsurancePolicyDto>>> HandleAsync(
        GetPoliciesByPatientQuery query,
        IInsurancePolicyRepository policyRepository,
        CancellationToken ct)
    {
        var policies = await policyRepository.GetActiveByPatientIdAsync(query.PatientId, ct);
        return Result.Success<IReadOnlyList<InsurancePolicyDto>>(
            policies.Select(p => p.ToDto()).ToList());
    }
}