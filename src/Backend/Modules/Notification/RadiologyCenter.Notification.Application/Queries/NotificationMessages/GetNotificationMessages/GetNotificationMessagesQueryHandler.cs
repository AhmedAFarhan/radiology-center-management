using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Application.DTOs;
using RadiologyCenter.Notification.Domain.Enumerations;

namespace RadiologyCenter.Notification.Application.Queries.NotificationMessages.GetNotificationMessages;

public static class GetNotificationMessagesQueryHandler
{
    public static async Task<Result<PagedResult<NotificationMessageDto>>> HandleAsync(
        GetNotificationMessagesQuery query,
        INotificationMessageRepository repository,
        CancellationToken ct)
    {
        var channel = string.IsNullOrWhiteSpace(query.Channel)
            ? null
            : NotificationChannel.GetAll<NotificationChannel>().FirstOrDefault(c => c.Name == query.Channel);

        var status = string.IsNullOrWhiteSpace(query.Status)
            ? null
            : NotificationStatus.GetAll<NotificationStatus>().FirstOrDefault(s => s.Name == query.Status);

        var paged = await repository.GetPagedAsync(query.Request, channel, status, ct);
        var items = paged.Items.Select(m => m.ToDto()).ToList();

        return Result.Success(new PagedResult<NotificationMessageDto>(
            items,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize));
    }
}