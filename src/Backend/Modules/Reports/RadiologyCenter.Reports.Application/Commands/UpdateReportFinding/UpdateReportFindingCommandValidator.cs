using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.UpdateReportFinding;

public class UpdateReportFindingCommandValidator : AbstractValidator<UpdateReportFindingCommand>
{
    public UpdateReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
        RuleFor(x => x.FindingId).NotEmpty().WithErrorCode(ErrorCodes.FindingIdRequired);
        RuleFor(x => x.Description).NotEmpty().WithErrorCode(ErrorCodes.DescriptionRequired).MaximumLength(5000).WithErrorCode(ErrorCodes.DescriptionTooLong);
        RuleFor(x => x.Severity).NotEmpty().WithErrorCode(ErrorCodes.SeverityRequired).IsEnumerationMember<FindingSeverity, UpdateReportFindingCommand>("Severity");
    }
}