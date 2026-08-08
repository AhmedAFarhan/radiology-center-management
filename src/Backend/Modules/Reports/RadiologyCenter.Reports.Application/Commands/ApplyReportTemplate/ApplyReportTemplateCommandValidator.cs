using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.ApplyReportTemplate;

public class ApplyReportTemplateCommandValidator : AbstractValidator<ApplyReportTemplateCommand>
{
    public ApplyReportTemplateCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.TemplateId).NotEmpty();
    }
}