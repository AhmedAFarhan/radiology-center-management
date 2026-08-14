using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Catalog.Application.Abstractions;

public interface IExaminationTypeRepository : IBaseRepository<ExaminationType, Guid>
{
    Task<ExaminationType?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ExaminationType>> GetWithItemsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<PagedResult<ExaminationType>> GetPagedWithItemsAsync(QueryRequest request, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken ct = default);
}
