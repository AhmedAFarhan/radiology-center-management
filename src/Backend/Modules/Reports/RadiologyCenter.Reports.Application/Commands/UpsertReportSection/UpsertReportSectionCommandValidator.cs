using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Application.Commands.UpsertReportSection;

public class UpsertReportSectionCommandValidator : AbstractValidator<UpsertReportSectionCommand>
{
    public UpsertReportSectionCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.SectionType).NotEmpty().IsEnumerationMember<ReportSectionType, UpsertReportSectionCommand>("SectionType");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).MaximumLength(10000);
        RuleFor(x => x.Position).GreaterThanOrEqualTo(0);
    }
}