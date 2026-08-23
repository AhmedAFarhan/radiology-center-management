using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.AddReportFinding;

public class AddReportFindingCommandValidator : AbstractValidator<AddReportFindingCommand>
{
    public AddReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Region).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Description).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(5000).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Severity).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<FindingSeverity, AddReportFindingCommand>("Severity");
        RuleFor(x => x.Position).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
    }
}