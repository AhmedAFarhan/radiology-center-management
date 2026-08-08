using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.UpdateReportFinding;

public class UpdateReportFindingCommandValidator : AbstractValidator<UpdateReportFindingCommand>
{
    public UpdateReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.FindingId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Severity).NotEmpty().IsEnumerationMember<FindingSeverity, UpdateReportFindingCommand>("Severity");
    }
}