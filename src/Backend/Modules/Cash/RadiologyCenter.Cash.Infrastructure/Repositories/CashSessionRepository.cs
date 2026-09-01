using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Domain.Entities;
using RadiologyCenter.Cash.Domain.Enumerations;
using RadiologyCenter.Cash.Infrastructure.Persistence;

namespace RadiologyCenter.Cash.Infrastructure.Repositories;

public class CashSessionRepository : BaseRepository<CashSession, Guid>, ICashSessionRepository
{
    public CashSessionRepository(CashDbContext context) : base(context) { }

    public async Task<CashSession?> GetOpenSessionByUserAsync(Guid userId, CancellationToken ct = default) =>
        await DbSet
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == CashSessionStatus.Open, ct);

    public async Task<PagedResult<CashSession>> GetPagedWithStatusAsync(
        QueryRequest request,
        CashSessionStatus? status,
        CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<CashSession>(FilterExpressionBuilder.Build<CashSession>(request.Filters));

        if (status is not null)
            spec.AddCriteria(s => s.Status == status);

        if (SearchExpressionBuilder.Build<CashSession>(request.SearchTerm, request.SearchFields) is { } searchCriteria)
            spec.AddCriteria(searchCriteria);

        if (SortExpressionBuilder.TryBuildSelector<CashSession>(request.SortBy, out var sortSelector))
        {
            if (request.SortDescending)
                spec.ApplyOrderByDescending(sortSelector);
            else
                spec.ApplyOrderBy(sortSelector);
        }

        var query = ApplySpecification(spec);
        var totalCount = await query.CountAsync(ct);

        spec.ApplyPaging((request.Pagination.PageNumber - 1) * request.Pagination.PageSize, request.Pagination.PageSize);
        var items = await ApplySpecification(spec).ToListAsync(ct);

        return PagedResult<CashSession>.Create(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }

    public async Task<IReadOnlyList<CashSession>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default) =>
        await DbSet
            .Where(s => s.OpenedAt >= from && s.OpenedAt < to)
            .OrderByDescending(s => s.OpenedAt)
            .ToListAsync(ct);
}