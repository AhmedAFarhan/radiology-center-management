using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Application.DTOs;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.UpdateNotificationTemplate;

public static class UpdateNotificationTemplateCommandHandler
{
    public static async Task<Result<NotificationTemplateDto>> HandleAsync(
        UpdateNotificationTemplateCommand command,
        INotificationTemplateRepository repository,
        INotificationUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await repository.GetByIdAsync(command.Id, ct);
        if (template is null)
            return Result.Failure<NotificationTemplateDto>(Error.NotFound("NotificationTemplate", command.Id));

        if (await repository.ExistsByCodeAsync(command.Code, command.Id, ct))
            return Result.Failure<NotificationTemplateDto>(Error.Conflict($"A template with code '{command.Code}' already exists."));

        template.Update(command.Name, command.Subject, command.Body);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(template.ToDto());
    }
}