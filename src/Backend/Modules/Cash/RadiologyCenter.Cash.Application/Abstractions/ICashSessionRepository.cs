using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Application.Abstractions;

public interface ICashSessionRepository : IBaseRepository<CashSession, Guid>
{
    Task<CashSession?> GetOpenSessionByUserAsync(Guid userId, CancellationToken ct = default);
    Task<PagedResult<CashSession>> GetPagedWithStatusAsync(QueryRequest request, CashSessionStatus? status, CancellationToken ct = default);
    Task<IReadOnlyList<CashSession>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
}