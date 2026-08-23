using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Catalog.Domain.Enumerations;
using RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;
using RadiologyCenter.Reports.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.UpdateReportTemplate;

public class UpdateReportTemplateCommandValidator : AbstractValidator<UpdateReportTemplateCommand>
{
    public UpdateReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Modality).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<Modality, UpdateReportTemplateCommand>("Modality");
        RuleFor(x => x.BodyPart).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.BodyPart));
        RuleFor(x => x.Description).MaximumLength(1000).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleForEach(x => x.Sections).ChildRules(sections =>
        {
            sections.RuleFor(s => s.SectionType).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired)
                .IsEnumerationMember<ReportSectionType, ReportTemplateSectionInput>("SectionType");
            sections.RuleFor(s => s.Title).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
            sections.RuleFor(s => s.Body).MaximumLength(10000).WithErrorCode(SharedCodes.Shared.TextTooLong);
            sections.RuleFor(s => s.Position).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        });
    }
}