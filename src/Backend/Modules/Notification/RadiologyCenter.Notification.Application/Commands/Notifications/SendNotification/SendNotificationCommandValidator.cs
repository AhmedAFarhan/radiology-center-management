using FluentValidation;

namespace RadiologyCenter.Notification.Application.Commands.Notifications.SendNotification;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Recipient).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Channel).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TemplateCode).MaximumLength(100);

        RuleFor(x => x.TemplateCode)
            .NotEmpty()
            .When(x => string.IsNullOrWhiteSpace(x.Subject) && string.IsNullOrWhiteSpace(x.Body))
            .WithMessage("Either TemplateCode or Subject/Body must be provided.");

        RuleFor(x => x.Subject).MaximumLength(500);
        RuleFor(x => x.ReferenceId).MaximumLength(64);
    }
}