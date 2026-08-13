using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class ClaimRepository : BaseRepository<Claim, Guid>, IClaimRepository
{
    public ClaimRepository(InsuranceDbContext context) : base(context) { }

    public override async Task<Claim?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(c => c.Settlements)
            .Include(c => c.Rejections)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public override async Task<IReadOnlyList<Claim>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet
            .Include(c => c.Settlements)
            .ToListAsync(ct);

    public override async Task<PagedResult<Claim>> GetPagedAsync(QueryRequest request, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<Claim>(FilterExpressionBuilder.Build<Claim>(request.Filters));

        if (SearchExpressionBuilder.Build<Claim>(request.SearchTerm, request.SearchFields) is { } searchCriteria)
            spec.AddCriteria(searchCriteria);

        spec.AddInclude(c => c.Settlements);

        if (SortExpressionBuilder.TryBuildSelector<Claim>(request.SortBy, out var sortSelector))
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

        return PagedResult<Claim>.Create(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }

    public async Task<Claim?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default)
    {
        var claim = await DbSet
            .FromSqlInterpolated($"SELECT * FROM [Insurance].[Claims] WITH (UPDLOCK, ROWLOCK) WHERE [Id] = {id}")
            .SingleOrDefaultAsync(ct);

        if (claim is null)
            return null;

        await DbSet.Entry(claim).Collection(c => c.Settlements).LoadAsync(ct);
        await DbSet.Entry(claim).Collection(c => c.Rejections).LoadAsync(ct);
        return claim;
    }

    public async Task<Claim?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default) =>
        await DbSet
            .Include(c => c.Settlements)
            .Include(c => c.Rejections)
            .FirstOrDefaultAsync(c => c.ExaminationId == examinationId, ct);
}