using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeactivateNotificationTemplate;

public class DeactivateNotificationTemplateCommandValidator : AbstractValidator<DeactivateNotificationTemplateCommand>
{
    public DeactivateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
    }
}