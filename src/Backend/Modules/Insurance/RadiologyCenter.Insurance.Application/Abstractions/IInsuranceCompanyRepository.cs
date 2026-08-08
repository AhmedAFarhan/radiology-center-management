using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Abstractions;

public interface IInsuranceCompanyRepository : IBaseRepository<InsuranceCompany, Guid>
{
    Task<InsuranceCompany?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}