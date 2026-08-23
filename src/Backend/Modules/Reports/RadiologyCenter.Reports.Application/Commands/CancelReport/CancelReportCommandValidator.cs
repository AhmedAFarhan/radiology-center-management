using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.CancelReport;

public class CancelReportCommandValidator : AbstractValidator<CancelReportCommand>
{
    public CancelReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Reason).MaximumLength(1000).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}