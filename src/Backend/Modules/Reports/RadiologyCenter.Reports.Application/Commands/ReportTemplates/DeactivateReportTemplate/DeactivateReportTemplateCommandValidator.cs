using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeactivateReportTemplate;

public class DeactivateReportTemplateCommandValidator : AbstractValidator<DeactivateReportTemplateCommand>
{
    public DeactivateReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(ErrorCodes.TemplateIdRequired);
    }
}