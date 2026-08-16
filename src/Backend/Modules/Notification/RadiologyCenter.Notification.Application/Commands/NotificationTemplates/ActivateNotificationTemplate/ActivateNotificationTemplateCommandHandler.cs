using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Notification.Application.Localization;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Application.DTOs;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.ActivateNotificationTemplate;

public static class ActivateNotificationTemplateCommandHandler
{
    public static async Task<Result<NotificationTemplateDto>> HandleAsync(
        ActivateNotificationTemplateCommand command,
        INotificationTemplateRepository repository,
        INotificationUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await repository.GetByIdAsync(command.Id, ct);
        if (template is null)
            return Result.Failure<NotificationTemplateDto>(Error.NotFound(ErrorCodes.TemplateNotFound, "NotificationTemplate", command.Id));

        template.Activate();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(template.ToDto());
    }
}