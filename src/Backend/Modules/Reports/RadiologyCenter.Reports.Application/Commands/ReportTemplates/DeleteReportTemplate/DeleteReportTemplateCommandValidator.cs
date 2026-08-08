using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeleteReportTemplate;

public class DeleteReportTemplateCommandValidator : AbstractValidator<DeleteReportTemplateCommand>
{
    public DeleteReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
    }
}