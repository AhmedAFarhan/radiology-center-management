using FluentValidation;
using RadiologyCenter.Notification.Application.Localization;

namespace RadiologyCenter.Notification.Application.Commands.Notifications.SendNotification;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Recipient).NotEmpty().WithErrorCode(ErrorCodes.RecipientRequired).MaximumLength(500).WithErrorCode(ErrorCodes.RecipientTooLong);
        RuleFor(x => x.Channel).NotEmpty().WithErrorCode(ErrorCodes.ChannelRequired).MaximumLength(50).WithErrorCode(ErrorCodes.ChannelTooLong);
        RuleFor(x => x.TemplateCode).MaximumLength(100).WithErrorCode(ErrorCodes.TemplateCodeTooLong);

        RuleFor(x => x.TemplateCode)
            .NotEmpty().WithErrorCode(ErrorCodes.TemplateCodeRequired)
            .When(x => string.IsNullOrWhiteSpace(x.Subject) && string.IsNullOrWhiteSpace(x.Body))
            .WithErrorCode(ErrorCodes.TemplateCodeOrBodyRequired);

        RuleFor(x => x.Subject).MaximumLength(500).WithErrorCode(ErrorCodes.SubjectTooLong);
        RuleFor(x => x.ReferenceId).MaximumLength(64).WithErrorCode(ErrorCodes.ReferenceIdTooLong);
    }
}
