using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Commands.FinalizeReport;

public class FinalizeReportCommandValidator : AbstractValidator<FinalizeReportCommand>
{
    public FinalizeReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
    }
}