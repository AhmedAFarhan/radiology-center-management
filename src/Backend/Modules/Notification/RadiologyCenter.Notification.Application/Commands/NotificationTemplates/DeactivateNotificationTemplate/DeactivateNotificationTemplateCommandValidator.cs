using FluentValidation;
using RadiologyCenter.Notification.Application.Localization;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeactivateNotificationTemplate;

public class DeactivateNotificationTemplateCommandValidator : AbstractValidator<DeactivateNotificationTemplateCommand>
{
    public DeactivateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.TemplateIdRequired);
    }
}
