using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Application.DTOs;

namespace RadiologyCenter.Notification.Application.Queries.NotificationTemplates.GetNotificationTemplateById;

public static class GetNotificationTemplateByIdQueryHandler
{
    public static async Task<Result<NotificationTemplateDto>> HandleAsync(
        GetNotificationTemplateByIdQuery query,
        INotificationTemplateRepository repository,
        CancellationToken ct)
    {
        var template = await repository.GetByIdAsync(query.Id, ct);
        if (template is null)
            return Result.Failure<NotificationTemplateDto>(Error.NotFound("NotificationTemplate", query.Id));

        return Result.Success(template.ToDto());
    }
}