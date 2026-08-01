using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IExaminationRepository : IBaseRepository<Examination, Guid>
{
    Task<Examination?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Examination>> GetPagedWithItemsAsync(QueryRequest request, CancellationToken ct = default);
    Task<bool> HasActiveExaminationsByTypeAsync(Guid examinationTypeId, CancellationToken ct = default);
}
