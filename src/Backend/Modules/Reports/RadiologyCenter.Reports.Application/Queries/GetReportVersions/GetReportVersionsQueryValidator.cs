using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Queries.GetReportVersions;

public class GetReportVersionsQueryValidator : AbstractValidator<GetReportVersionsQuery>
{
    public GetReportVersionsQueryValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}