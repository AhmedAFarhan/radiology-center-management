using FluentValidation;
using RadiologyCenter.Notification.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Notification.Application.Commands.Notifications.SendNotification;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Recipient).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Channel).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(50).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.TemplateCode).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);

        RuleFor(x => x.TemplateCode)
            .NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired)
            .When(x => string.IsNullOrWhiteSpace(x.Subject) && string.IsNullOrWhiteSpace(x.Body))
            .WithErrorCode(ErrorCodes.TemplateCodeOrBodyRequired);

        RuleFor(x => x.Subject).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.ReferenceId).MaximumLength(64).WithErrorCode(SharedCodes.Shared.TextTooLong);
    }
}