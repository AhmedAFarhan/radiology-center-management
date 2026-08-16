using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Notification.Application.Localization;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Application.DTOs;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.CreateNotificationTemplate;

public static class CreateNotificationTemplateCommandHandler
{
    public static async Task<Result<NotificationTemplateDto>> HandleAsync(
        CreateNotificationTemplateCommand command,
        INotificationTemplateRepository repository,
        INotificationUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (await repository.ExistsByCodeAsync(command.Code, ct: ct))
            return Result.Failure<NotificationTemplateDto>(Error.Conflict(ErrorCodes.TemplateCodeExists, $"A template with code '{command.Code}' already exists."));

        var template = new NotificationTemplate(command.Code, command.Name, command.Subject, command.Body);

        await repository.AddAsync(template, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(template.ToDto());
    }
}