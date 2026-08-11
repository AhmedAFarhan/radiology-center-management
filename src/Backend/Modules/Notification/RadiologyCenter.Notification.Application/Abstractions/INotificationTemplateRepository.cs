using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Notification.Domain.Entities;

namespace RadiologyCenter.Notification.Application.Abstractions;

public interface INotificationTemplateRepository : IBaseRepository<NotificationTemplate, Guid>
{
    Task<NotificationTemplate?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken ct = default);
    new Task<PagedResult<NotificationTemplate>> GetPagedAsync(QueryRequest request, CancellationToken ct = default);
}