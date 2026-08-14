using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Domain.Entities;
using RadiologyCenter.Catalog.Infrastructure.Persistence;

namespace RadiologyCenter.Catalog.Infrastructure.Repositories;

public class ExaminationTypeRepository : BaseRepository<ExaminationType, Guid>, IExaminationTypeRepository
{
    public ExaminationTypeRepository(CatalogDbContext context) : base(context) { }

    public async Task<ExaminationType?> GetWithItemsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<ExaminationType>> GetWithItemsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        return await DbSet
            .Include(t => t.Items)
            .Where(t => idList.Contains(t.Id))
            .ToListAsync(ct);
    }

    public async Task<PagedResult<ExaminationType>> GetPagedWithItemsAsync(QueryRequest request, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<ExaminationType>(FilterExpressionBuilder.Build<ExaminationType>(request.Filters));

        if (SearchExpressionBuilder.Build<ExaminationType>(request.SearchTerm, request.SearchFields) is { } searchCriteria)
            spec.AddCriteria(searchCriteria);

        if (SortExpressionBuilder.TryBuildSelector<ExaminationType>(request.SortBy, out var sortSelector))
        {
            if (request.SortDescending)
                spec.ApplyOrderByDescending(sortSelector);
            else
                spec.ApplyOrderBy(sortSelector);
        }

        var query = ApplySpecification(spec).Include(t => t.Items);
        var totalCount = await query.CountAsync(ct);

        spec.ApplyPaging((request.Pagination.PageNumber - 1) * request.Pagination.PageSize, request.Pagination.PageSize);
        var items = await ApplySpecification(spec).Include(t => t.Items).ToListAsync(ct);

        return PagedResult<ExaminationType>.Create(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }

    public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken ct = default)
    {
        if (excludeId.HasValue)
            return await DbSet.AnyAsync(t => t.Code == code && t.Id != excludeId.Value, ct);
        return await DbSet.AnyAsync(t => t.Code == code, ct);
    }

    public override async Task<ExaminationType?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(t => t.Id == id, ct);
}
