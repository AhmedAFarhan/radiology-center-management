using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.AddReportFinding;

public class AddReportFindingCommandValidator : AbstractValidator<AddReportFindingCommand>
{
    public AddReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
        RuleFor(x => x.Region).NotEmpty().WithErrorCode(ErrorCodes.RegionRequired).MaximumLength(200).WithErrorCode(ErrorCodes.RegionTooLong);
        RuleFor(x => x.Description).NotEmpty().WithErrorCode(ErrorCodes.DescriptionRequired).MaximumLength(5000).WithErrorCode(ErrorCodes.DescriptionTooLong);
        RuleFor(x => x.Severity).NotEmpty().WithErrorCode(ErrorCodes.SeverityRequired).IsEnumerationMember<FindingSeverity, AddReportFindingCommand>("Severity");
        RuleFor(x => x.Position).GreaterThanOrEqualTo(0).WithErrorCode("Shared.CannotBeNegative");
    }
}