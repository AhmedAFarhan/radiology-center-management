using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.UpdateReportFinding;

public class UpdateReportFindingCommandValidator : AbstractValidator<UpdateReportFindingCommand>
{
    public UpdateReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.FindingId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Description).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(5000).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Severity).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<FindingSeverity, UpdateReportFindingCommand>("Severity");
    }
}