using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Abstractions;

public interface IInsurancePolicyRepository : IBaseRepository<InsurancePolicy, Guid>
{
    Task<IReadOnlyList<InsurancePolicy>> GetActiveByPatientIdAsync(Guid patientId, CancellationToken ct = default);
    Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken ct = default);
}