using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.AddTemplateSection;

public class AddTemplateSectionCommandValidator : AbstractValidator<AddTemplateSectionCommand>
{
    public AddTemplateSectionCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
        RuleFor(x => x.Section).NotNull().ChildRules(sections =>
        {
            sections.RuleFor(s => s!.SectionType).NotEmpty()
                .IsEnumerationMember<ReportSectionType, ReportTemplateSectionInput>("SectionType");
            sections.RuleFor(s => s!.Title).NotEmpty().MaximumLength(200);
            sections.RuleFor(s => s!.Body).MaximumLength(10000);
            sections.RuleFor(s => s!.Position).GreaterThanOrEqualTo(0);
        });
    }
}