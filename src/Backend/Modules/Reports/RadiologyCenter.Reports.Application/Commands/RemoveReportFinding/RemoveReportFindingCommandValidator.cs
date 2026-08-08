using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.RemoveReportFinding;

public class RemoveReportFindingCommandValidator : AbstractValidator<RemoveReportFindingCommand>
{
    public RemoveReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.FindingId).NotEmpty();
    }
}