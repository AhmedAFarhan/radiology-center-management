using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Abstractions;

public interface IClaimRepository : IBaseRepository<Claim, Guid>
{
    Task<Claim?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default);
    Task<Claim?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default);
}