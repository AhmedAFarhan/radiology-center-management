using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.DeleteNotificationTemplate;

public class DeleteNotificationTemplateCommandValidator : AbstractValidator<DeleteNotificationTemplateCommand>
{
    public DeleteNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
    }
}