using FluentValidation;
using RadiologyCenter.Notification.Application.Localization;

namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.CreateNotificationTemplate;

public class CreateNotificationTemplateCommandValidator : AbstractValidator<CreateNotificationTemplateCommand>
{
    public CreateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithErrorCode(ErrorCodes.TemplateCodeRequired).MaximumLength(100).WithErrorCode(ErrorCodes.TemplateCodeTooLong);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.TemplateNameRequired).MaximumLength(200).WithErrorCode(ErrorCodes.TemplateNameTooLong);
        RuleFor(x => x.Subject).NotEmpty().WithErrorCode(ErrorCodes.TemplateSubjectRequired).MaximumLength(400).WithErrorCode(ErrorCodes.TemplateSubjectTooLong);
        RuleFor(x => x.Body).NotEmpty().WithErrorCode(ErrorCodes.TemplateBodyRequired);
    }
}
