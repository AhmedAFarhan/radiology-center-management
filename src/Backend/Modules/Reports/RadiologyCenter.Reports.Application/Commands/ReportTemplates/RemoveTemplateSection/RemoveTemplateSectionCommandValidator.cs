using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.RemoveTemplateSection;

public class RemoveTemplateSectionCommandValidator : AbstractValidator<RemoveTemplateSectionCommand>
{
    public RemoveTemplateSectionCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.SectionId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}