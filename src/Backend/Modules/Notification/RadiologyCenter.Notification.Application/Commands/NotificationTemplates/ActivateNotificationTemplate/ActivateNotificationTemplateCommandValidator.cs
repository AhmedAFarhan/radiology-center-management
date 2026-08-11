using FluentValidation;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.ActivateNotificationTemplate;

public class ActivateNotificationTemplateCommandValidator : AbstractValidator<ActivateNotificationTemplateCommand>
{
    public ActivateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}