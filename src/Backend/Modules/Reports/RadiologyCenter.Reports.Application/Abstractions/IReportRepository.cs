using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Abstractions;

public interface IReportRepository : IBaseRepository<RadiologyReport, Guid>
{
    Task<RadiologyReport?> GetByIdWithVersionsAsync(Guid id, CancellationToken ct = default);
    Task<RadiologyReport?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default);
    Task<bool> HasReportByExaminationAsync(Guid examinationId, CancellationToken ct = default);
}