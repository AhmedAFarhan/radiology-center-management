using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Commands.RemoveReportFinding;

public class RemoveReportFindingCommandValidator : AbstractValidator<RemoveReportFindingCommand>
{
    public RemoveReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
        RuleFor(x => x.FindingId).NotEmpty().WithErrorCode(ErrorCodes.FindingIdRequired);
    }
}