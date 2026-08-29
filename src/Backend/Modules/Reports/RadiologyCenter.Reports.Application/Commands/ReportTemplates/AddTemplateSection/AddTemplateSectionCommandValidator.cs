using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.AddTemplateSection;

public class AddTemplateSectionCommandValidator : AbstractValidator<AddTemplateSectionCommand>
{
    public AddTemplateSectionCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(ErrorCodes.TemplateIdRequired);
        RuleFor(x => x.Section).NotNull().WithErrorCode(ErrorCodes.SectionRequired).ChildRules(sections =>
        {
            sections.RuleFor(s => s!.SectionType).NotEmpty().WithErrorCode(ErrorCodes.SectionTypeRequired)
                .IsEnumerationMember<ReportSectionType, ReportTemplateSectionInput>("SectionType");
            sections.RuleFor(s => s!.Title).NotEmpty().WithErrorCode(ErrorCodes.TitleRequired).MaximumLength(200).WithErrorCode(ErrorCodes.TitleTooLong);
            sections.RuleFor(s => s!.Body).MaximumLength(10000).WithErrorCode(ErrorCodes.BodyTooLong);
            sections.RuleFor(s => s!.Position).GreaterThanOrEqualTo(0).WithErrorCode("Shared.CannotBeNegative");
        });
    }
}