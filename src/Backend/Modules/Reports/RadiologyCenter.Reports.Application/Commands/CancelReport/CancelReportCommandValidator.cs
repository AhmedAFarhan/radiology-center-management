using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.CancelReport;

public class CancelReportCommandValidator : AbstractValidator<CancelReportCommand>
{
    public CancelReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}