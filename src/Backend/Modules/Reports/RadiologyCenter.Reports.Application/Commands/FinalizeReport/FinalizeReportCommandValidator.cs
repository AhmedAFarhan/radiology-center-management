using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.FinalizeReport;

public class FinalizeReportCommandValidator : AbstractValidator<FinalizeReportCommand>
{
    public FinalizeReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}