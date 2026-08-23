using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.UpsertReportSection;

public class UpsertReportSectionCommandValidator : AbstractValidator<UpsertReportSectionCommand>
{
    public UpsertReportSectionCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.SectionType).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<ReportSectionType, UpsertReportSectionCommand>("SectionType");
        RuleFor(x => x.Title).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Body).MaximumLength(10000).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Position).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
    }
}