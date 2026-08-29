using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Commands.CancelReport;

public class CancelReportCommandValidator : AbstractValidator<CancelReportCommand>
{
    public CancelReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
        RuleFor(x => x.Reason).MaximumLength(1000).WithErrorCode(ErrorCodes.DescriptionTooLong).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}