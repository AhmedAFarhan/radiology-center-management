using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Notification.Application.Abstractions;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeleteNotificationTemplate;

public static class DeleteNotificationTemplateCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteNotificationTemplateCommand command,
        INotificationTemplateRepository repository,
        INotificationUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await repository.GetByIdAsync(command.Id, ct);
        if (template is null)
            return Result.Failure(Error.NotFound("NotificationTemplate", command.Id));

        repository.Remove(template);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}