using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.AmendReport;

public class AmendReportCommandValidator : AbstractValidator<AmendReportCommand>
{
    public AmendReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}