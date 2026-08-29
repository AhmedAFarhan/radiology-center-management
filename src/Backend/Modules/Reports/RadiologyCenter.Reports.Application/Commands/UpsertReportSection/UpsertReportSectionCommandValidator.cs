using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.UpsertReportSection;

public class UpsertReportSectionCommandValidator : AbstractValidator<UpsertReportSectionCommand>
{
    public UpsertReportSectionCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
        RuleFor(x => x.SectionType).NotEmpty().WithErrorCode(ErrorCodes.SectionTypeRequired).IsEnumerationMember<ReportSectionType, UpsertReportSectionCommand>("SectionType");
        RuleFor(x => x.Title).NotEmpty().WithErrorCode(ErrorCodes.TitleRequired).MaximumLength(200).WithErrorCode(ErrorCodes.TitleTooLong);
        RuleFor(x => x.Body).MaximumLength(10000).WithErrorCode(ErrorCodes.BodyTooLong);
        RuleFor(x => x.Position).GreaterThanOrEqualTo(0).WithErrorCode("Shared.CannotBeNegative");
    }
}