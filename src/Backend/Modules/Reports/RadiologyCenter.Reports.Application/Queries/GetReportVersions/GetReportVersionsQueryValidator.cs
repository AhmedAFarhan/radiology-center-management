using FluentValidation;

namespace RadiologyCenter.Reports.Application.Queries.GetReportVersions;

public class GetReportVersionsQueryValidator : AbstractValidator<GetReportVersionsQuery>
{
    public GetReportVersionsQueryValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
    }
}