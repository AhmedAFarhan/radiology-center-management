using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.ActivateNotificationTemplate;

public class ActivateNotificationTemplateCommandValidator : AbstractValidator<ActivateNotificationTemplateCommand>
{
    public ActivateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
    }
}