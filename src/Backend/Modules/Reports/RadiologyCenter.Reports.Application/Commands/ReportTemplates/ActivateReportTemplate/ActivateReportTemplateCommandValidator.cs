using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.ActivateReportTemplate;

public class ActivateReportTemplateCommandValidator : AbstractValidator<ActivateReportTemplateCommand>
{
    public ActivateReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
    }
}