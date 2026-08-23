using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;
using RadiologyCenter.Reports.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.AddTemplateSection;

public class AddTemplateSectionCommandValidator : AbstractValidator<AddTemplateSectionCommand>
{
    public AddTemplateSectionCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Section).NotNull().WithErrorCode(SharedCodes.Shared.FieldRequired).ChildRules(sections =>
        {
            sections.RuleFor(s => s!.SectionType).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired)
                .IsEnumerationMember<ReportSectionType, ReportTemplateSectionInput>("SectionType");
            sections.RuleFor(s => s!.Title).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
            sections.RuleFor(s => s!.Body).MaximumLength(10000).WithErrorCode(SharedCodes.Shared.TextTooLong);
            sections.RuleFor(s => s!.Position).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        });
    }
}