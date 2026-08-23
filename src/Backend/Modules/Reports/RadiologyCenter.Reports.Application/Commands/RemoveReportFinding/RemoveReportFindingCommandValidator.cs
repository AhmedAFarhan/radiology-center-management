using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.RemoveReportFinding;

public class RemoveReportFindingCommandValidator : AbstractValidator<RemoveReportFindingCommand>
{
    public RemoveReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.FindingId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}