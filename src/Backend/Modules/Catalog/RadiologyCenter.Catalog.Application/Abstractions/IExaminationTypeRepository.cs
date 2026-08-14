using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Catalog.Application.Abstractions;

public interface IExaminationTypeRepository : IBaseRepository<ExaminationType, Guid>
{
    Task<PagedResult<ExaminationType>> GetPagedAsync(QueryRequest request, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken ct = default);
}
