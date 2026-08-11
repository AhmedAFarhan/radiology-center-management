using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Domain.Entities;
using RadiologyCenter.Notification.Infrastructure.Persistence;

namespace RadiologyCenter.Notification.Infrastructure.Repositories;

public class NotificationTemplateRepository : BaseRepository<NotificationTemplate, Guid>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(NotificationDbContext context) : base(context) { }

    public async Task<NotificationTemplate?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(t => t.Code == code, ct);

    public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken ct = default) =>
        await DbSet.AnyAsync(t => t.Code == code && (excludeId == null || t.Id != excludeId), ct);

    public async new Task<PagedResult<NotificationTemplate>> GetPagedAsync(QueryRequest request, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<NotificationTemplate>(FilterExpressionBuilder.Build<NotificationTemplate>(request.Filters));

        if (SearchExpressionBuilder.Build<NotificationTemplate>(request.SearchTerm, request.SearchFields) is { } searchCriteria)
            spec.AddCriteria(searchCriteria);

        if (SortExpressionBuilder.TryBuildSelector<NotificationTemplate>(request.SortBy, out var sortSelector))
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

        return PagedResult<NotificationTemplate>.Create(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }
}