using FluentValidation;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.UpdateNotificationTemplate;

public class UpdateNotificationTemplateCommandValidator : AbstractValidator<UpdateNotificationTemplateCommand>
{
    public UpdateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(400);
        RuleFor(x => x.Body).NotEmpty();
    }
}