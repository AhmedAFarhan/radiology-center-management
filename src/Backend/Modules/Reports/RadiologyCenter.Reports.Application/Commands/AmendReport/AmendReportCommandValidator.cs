using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.AmendReport;

public class AmendReportCommandValidator : AbstractValidator<AmendReportCommand>
{
    public AmendReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Reason).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(1000).WithErrorCode(SharedCodes.Shared.TextTooLong);
    }
}