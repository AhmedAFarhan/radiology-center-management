using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Notification.Application.Queries.NotificationTemplates.GetNotificationTemplates;

public record GetNotificationTemplatesQuery(QueryRequest Request) : IQuery;