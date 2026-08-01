using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IVisitRepository : IBaseRepository<Visit, Guid>
{
    Task<Visit?> GetWithExaminationsAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Visit>> GetPagedWithExaminationsAsync(QueryRequest request, CancellationToken ct = default);
}
