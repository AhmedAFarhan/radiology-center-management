using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Commands.AmendReport;

public class AmendReportCommandValidator : AbstractValidator<AmendReportCommand>
{
    public AmendReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
        RuleFor(x => x.Reason).NotEmpty().WithErrorCode(ErrorCodes.DescriptionRequired).MaximumLength(1000).WithErrorCode(ErrorCodes.DescriptionTooLong);
    }
}