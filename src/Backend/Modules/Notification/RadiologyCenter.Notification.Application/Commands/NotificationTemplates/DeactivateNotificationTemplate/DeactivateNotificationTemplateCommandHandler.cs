using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Notification.Application.Abstractions;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeactivateNotificationTemplate;

public static class DeactivateNotificationTemplateCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateNotificationTemplateCommand command,
        INotificationTemplateRepository repository,
        INotificationUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await repository.GetByIdAsync(command.Id, ct);
        if (template is null)
            return Result.Failure(Error.NotFound("NotificationTemplate", command.Id));

        template.Deactivate();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}