using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Catalog.Domain.Enumerations;
using RadiologyCenter.Reports.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.CreateReportTemplate;

public class CreateReportTemplateCommandValidator : AbstractValidator<CreateReportTemplateCommand>
{
    public CreateReportTemplateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Modality).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).IsEnumerationMember<Modality, CreateReportTemplateCommand>("Modality");
        RuleFor(x => x.BodyPart).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.BodyPart));
        RuleFor(x => x.Description).MaximumLength(1000).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleForEach(x => x.Sections).ChildRules(sections =>
        {
            sections.RuleFor(s => s.SectionType).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired)
                .IsEnumerationMember<ReportSectionType, ReportTemplateSectionInput>("SectionType");
            sections.RuleFor(s => s.Title).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong);
            sections.RuleFor(s => s.Body).MaximumLength(10000).WithErrorCode(ErrorCodes.Shared.TextTooLong);
            sections.RuleFor(s => s.Position).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative);
        });
    }
}