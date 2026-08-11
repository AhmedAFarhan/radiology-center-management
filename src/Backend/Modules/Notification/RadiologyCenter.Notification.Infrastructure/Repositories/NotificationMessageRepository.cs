using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Domain.Entities;
using RadiologyCenter.Notification.Domain.Enumerations;
using RadiologyCenter.Notification.Infrastructure.Persistence;

namespace RadiologyCenter.Notification.Infrastructure.Repositories;

public class NotificationMessageRepository : BaseRepository<NotificationMessage, Guid>, INotificationMessageRepository
{
    public NotificationMessageRepository(NotificationDbContext context) : base(context) { }

    public async new Task<PagedResult<NotificationMessage>> GetPagedAsync(
        QueryRequest request,
        NotificationChannel? channel = null,
        NotificationStatus? status = null,
        CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<NotificationMessage>(FilterExpressionBuilder.Build<NotificationMessage>(request.Filters));

        if (channel is not null)
            spec.AddCriteria(m => m.Channel == channel);

        if (status is not null)
            spec.AddCriteria(m => m.Status == status);

        if (SearchExpressionBuilder.Build<NotificationMessage>(request.SearchTerm, request.SearchFields) is { } searchCriteria)
            spec.AddCriteria(searchCriteria);

        if (SortExpressionBuilder.TryBuildSelector<NotificationMessage>(request.SortBy, out var sortSelector))
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

        return PagedResult<NotificationMessage>.Create(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }
}