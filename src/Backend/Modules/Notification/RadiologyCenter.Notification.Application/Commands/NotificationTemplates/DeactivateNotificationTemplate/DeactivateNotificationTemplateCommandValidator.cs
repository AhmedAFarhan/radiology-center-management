using FluentValidation;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeactivateNotificationTemplate;

public class DeactivateNotificationTemplateCommandValidator : AbstractValidator<DeactivateNotificationTemplateCommand>
{
    public DeactivateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}