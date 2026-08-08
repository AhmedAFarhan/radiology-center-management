using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.RemoveTemplateSection;

public class RemoveTemplateSectionCommandValidator : AbstractValidator<RemoveTemplateSectionCommand>
{
    public RemoveTemplateSectionCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
    }
}