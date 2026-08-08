using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.FinalizeReport;

public class FinalizeReportCommandValidator : AbstractValidator<FinalizeReportCommand>
{
    public FinalizeReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
    }
}