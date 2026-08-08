using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Abstractions;

public interface IReportTemplateRepository : IBaseRepository<ReportTemplate, Guid>
{
    Task<ReportTemplate?> GetByIdWithSectionsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
}