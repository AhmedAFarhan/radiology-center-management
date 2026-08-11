using FluentValidation;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeleteNotificationTemplate;

public class DeleteNotificationTemplateCommandValidator : AbstractValidator<DeleteNotificationTemplateCommand>
{
    public DeleteNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}