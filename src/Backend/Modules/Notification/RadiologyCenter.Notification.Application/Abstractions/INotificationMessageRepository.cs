using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Notification.Domain.Entities;
using RadiologyCenter.Notification.Domain.Enumerations;

namespace RadiologyCenter.Notification.Application.Abstractions;

public interface INotificationMessageRepository : IBaseRepository<NotificationMessage, Guid>
{
    new Task<PagedResult<NotificationMessage>> GetPagedAsync(QueryRequest request, CancellationToken ct = default);
    Task<PagedResult<NotificationMessage>> GetPagedAsync(
        QueryRequest request,
        NotificationChannel? channel,
        NotificationStatus? status,
        CancellationToken ct = default);
}