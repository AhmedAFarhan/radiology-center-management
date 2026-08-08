using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.UpdateReportTemplate;

public class UpdateReportTemplateCommandValidator : AbstractValidator<UpdateReportTemplateCommand>
{
    public UpdateReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Modality).NotEmpty().IsEnumerationMember<Modality, UpdateReportTemplateCommand>("Modality");
        RuleFor(x => x.BodyPart).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.BodyPart));
        RuleFor(x => x.Description).MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleForEach(x => x.Sections).ChildRules(sections =>
        {
            sections.RuleFor(s => s.SectionType).NotEmpty()
                .IsEnumerationMember<ReportSectionType, ReportTemplateSectionInput>("SectionType");
            sections.RuleFor(s => s.Title).NotEmpty().MaximumLength(200);
            sections.RuleFor(s => s.Body).MaximumLength(10000);
            sections.RuleFor(s => s.Position).GreaterThanOrEqualTo(0);
        });
    }
}