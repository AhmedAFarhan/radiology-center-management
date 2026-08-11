using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Notification.Application.Queries.NotificationMessages.GetNotificationMessages;

public record GetNotificationMessagesQuery(
    QueryRequest Request,
    string? Channel = null,
    string? Status = null) : IQuery;