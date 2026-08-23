using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Queries.GetReportById;

public class GetReportByIdQueryValidator : AbstractValidator<GetReportByIdQuery>
{
    public GetReportByIdQueryValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}