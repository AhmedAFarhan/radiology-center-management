using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Commands.ApplyReportTemplate;

public class ApplyReportTemplateCommandValidator : AbstractValidator<ApplyReportTemplateCommand>
{
    public ApplyReportTemplateCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(ErrorCodes.TemplateIdRequired);
    }
}