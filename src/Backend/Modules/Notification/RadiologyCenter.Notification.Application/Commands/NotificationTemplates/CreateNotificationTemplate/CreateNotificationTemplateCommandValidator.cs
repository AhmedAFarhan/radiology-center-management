using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.CreateNotificationTemplate;

public class CreateNotificationTemplateCommandValidator : AbstractValidator<CreateNotificationTemplateCommand>
{
    public CreateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Subject).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(400).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Body).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
    }
}