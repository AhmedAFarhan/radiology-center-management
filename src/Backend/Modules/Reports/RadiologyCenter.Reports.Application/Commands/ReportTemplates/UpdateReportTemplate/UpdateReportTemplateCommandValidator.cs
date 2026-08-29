using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Catalog.Domain.Enumerations;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.UpdateReportTemplate;

public class UpdateReportTemplateCommandValidator : AbstractValidator<UpdateReportTemplateCommand>
{
    public UpdateReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(ErrorCodes.TemplateIdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.NameRequired).MaximumLength(200).WithErrorCode(ErrorCodes.NameTooLong);
        RuleFor(x => x.Modality).NotEmpty().WithErrorCode(ErrorCodes.ModalityRequired).IsEnumerationMember<Modality, UpdateReportTemplateCommand>("Modality");
        RuleFor(x => x.BodyPart).MaximumLength(200).WithErrorCode(ErrorCodes.BodyPartTooLong).When(x => !string.IsNullOrWhiteSpace(x.BodyPart));
        RuleFor(x => x.Description).MaximumLength(1000).WithErrorCode(ErrorCodes.DescriptionTooLong).When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleForEach(x => x.Sections).ChildRules(sections =>
        {
            sections.RuleFor(s => s.SectionType).NotEmpty().WithErrorCode(ErrorCodes.SectionTypeRequired)
                .IsEnumerationMember<ReportSectionType, ReportTemplateSectionInput>("SectionType");
            sections.RuleFor(s => s.Title).NotEmpty().WithErrorCode(ErrorCodes.TitleRequired).MaximumLength(200).WithErrorCode(ErrorCodes.TitleTooLong);
            sections.RuleFor(s => s.Body).MaximumLength(10000).WithErrorCode(ErrorCodes.BodyTooLong);
            sections.RuleFor(s => s.Position).GreaterThanOrEqualTo(0).WithErrorCode("Shared.CannotBeNegative");
        });
    }
}