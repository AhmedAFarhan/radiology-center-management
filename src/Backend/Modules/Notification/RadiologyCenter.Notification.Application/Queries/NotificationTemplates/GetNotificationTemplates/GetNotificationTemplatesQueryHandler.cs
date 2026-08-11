using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Application.DTOs;

namespace RadiologyCenter.Notification.Application.Queries.NotificationTemplates.GetNotificationTemplates;

public static class GetNotificationTemplatesQueryHandler
{
    public static async Task<Result<PagedResult<NotificationTemplateDto>>> HandleAsync(
        GetNotificationTemplatesQuery query,
        INotificationTemplateRepository repository,
        CancellationToken ct)
    {
        var paged = await repository.GetPagedAsync(query.Request, ct);
        var items = paged.Items.Select(t => t.ToDto()).ToList();

        return Result.Success(new PagedResult<NotificationTemplateDto>(
            items,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}