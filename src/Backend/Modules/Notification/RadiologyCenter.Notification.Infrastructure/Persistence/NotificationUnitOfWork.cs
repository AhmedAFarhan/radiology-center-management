using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Notification.Application.Abstractions;

namespace RadiologyCenter.Notification.Infrastructure.Persistence;

public class NotificationUnitOfWork : UnitOfWork<NotificationDbContext>, INotificationUnitOfWork
{
    public NotificationUnitOfWork(NotificationDbContext context, IDomainEventDispatcher eventDispatcher)
        : base(context, eventDispatcher)
    {
    }
}