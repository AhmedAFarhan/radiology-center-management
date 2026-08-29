using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.RemoveTemplateSection;

public class RemoveTemplateSectionCommandValidator : AbstractValidator<RemoveTemplateSectionCommand>
{
    public RemoveTemplateSectionCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(ErrorCodes.TemplateIdRequired);
        RuleFor(x => x.SectionId).NotEmpty().WithErrorCode(ErrorCodes.FindingIdRequired);
    }
}