using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Infrastructure.Persistence;

namespace RadiologyCenter.Examinations.Infrastructure.Repositories;

public class ExaminationRepository : BaseRepository<Examination, Guid>, IExaminationRepository
{
    public ExaminationRepository(ExaminationsDbContext context) : base(context) { }

    public override async Task<Examination?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Examination?> GetWithItemsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<Examination>> GetPagedWithItemsAsync(QueryRequest request, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<Examination>(FilterExpressionBuilder.Build<Examination>(request.Filters));

        if (SearchExpressionBuilder.Build<Examination>(request.SearchTerm, request.SearchFields) is { } searchCriteria)
            spec.AddCriteria(searchCriteria);

        if (SortExpressionBuilder.TryBuildSelector<Examination>(request.SortBy, out var sortSelector))
        {
            if (request.SortDescending)
                spec.ApplyOrderByDescending(sortSelector);
            else
                spec.ApplyOrderBy(sortSelector);
        }

        var query = ApplySpecification(spec).Include(e => e.Items);
        var totalCount = await query.CountAsync(ct);

        spec.ApplyPaging((request.Pagination.PageNumber - 1) * request.Pagination.PageSize, request.Pagination.PageSize);
        var items = await ApplySpecification(spec).Include(e => e.Items).ToListAsync(ct);

        return PagedResult<Examination>.Create(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }

    public async Task<bool> HasActiveExaminationsByTypeAsync(Guid examinationTypeId, CancellationToken ct = default) =>
        await DbSet.AnyAsync(
            e => e.ExaminationTypeId == examinationTypeId
                 && e.Status != ExaminationStatus.Completed
                 && e.Status != ExaminationStatus.Cancelled,
            ct);

    public async Task<IReadOnlyList<ExamFinancialProjection>> GetFinancialProjectionAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var query = DbSet.Where(e => e.Status == ExaminationStatus.Completed);

        if (from is not null)
            query = query.Where(e => e.CompletedAt >= from);

        if (to is not null)
            query = query.Where(e => e.CompletedAt <= to);

        return await query
            .Select(e => new ExamFinancialProjection(
                e.Id,
                e.ExaminationTypeId,
                e.CompletedAt,
                e.Price,
                e.Discount,
                e.IsDiscountPercentage,
                e.Paid,
                e.Remaining))
            .ToListAsync(ct);
    }
}
