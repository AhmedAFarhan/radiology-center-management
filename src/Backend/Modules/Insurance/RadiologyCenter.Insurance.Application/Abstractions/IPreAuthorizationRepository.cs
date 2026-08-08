using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Insurance.Application.Abstractions;

public interface IPreAuthorizationRepository : IBaseRepository<PreAuthorization, Guid>
{
    Task<PreAuthorization?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default);
}