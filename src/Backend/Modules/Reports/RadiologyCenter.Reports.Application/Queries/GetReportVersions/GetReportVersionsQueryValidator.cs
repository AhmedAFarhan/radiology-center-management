using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Queries.GetReportVersions;

public class GetReportVersionsQueryValidator : AbstractValidator<GetReportVersionsQuery>
{
    public GetReportVersionsQueryValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(ErrorCodes.ReportIdRequired);
    }
}