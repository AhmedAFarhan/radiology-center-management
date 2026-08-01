using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Infrastructure.Persistence;

namespace RadiologyCenter.Examinations.Infrastructure.Repositories;

public class VisitRepository : BaseRepository<Visit, Guid>, IVisitRepository
{
    public VisitRepository(ExaminationsDbContext context) : base(context) { }

    public async Task<Visit?> GetWithExaminationsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(v => v.Examinations)
                .ThenInclude(e => e.Items)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<PagedResult<Visit>> GetPagedWithExaminationsAsync(QueryRequest request, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<Visit>(FilterExpressionBuilder.Build<Visit>(request.Filters));

        if (SearchExpressionBuilder.Build<Visit>(request.SearchTerm, request.SearchFields) is { } searchCriteria)
            spec.AddCriteria(searchCriteria);

        if (SortExpressionBuilder.TryBuildSelector<Visit>(request.SortBy, out var sortSelector))
        {
            if (request.SortDescending)
                spec.ApplyOrderByDescending(sortSelector);
            else
                spec.ApplyOrderBy(sortSelector);
        }

        var query = ApplySpecification(spec).Include(v => v.Examinations).ThenInclude(e => e.Items);
        var totalCount = await query.CountAsync(ct);

        spec.ApplyPaging((request.Pagination.PageNumber - 1) * request.Pagination.PageSize, request.Pagination.PageSize);
        var items = await ApplySpecification(spec).Include(v => v.Examinations).ThenInclude(e => e.Items).ToListAsync(ct);

        return PagedResult<Visit>.Create(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }

    public override async Task<Visit?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(v => v.Id == id, ct);
}
