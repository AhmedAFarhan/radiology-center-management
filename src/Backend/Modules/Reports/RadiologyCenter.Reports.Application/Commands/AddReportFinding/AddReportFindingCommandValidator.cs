using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.AddReportFinding;

public class AddReportFindingCommandValidator : AbstractValidator<AddReportFindingCommand>
{
    public AddReportFindingCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.Region).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Severity).NotEmpty().IsEnumerationMember<FindingSeverity, AddReportFindingCommand>("Severity");
        RuleFor(x => x.Position).GreaterThanOrEqualTo(0);
    }
}