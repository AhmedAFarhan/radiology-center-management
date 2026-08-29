using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Queries.GetReportTemplates;

public class GetReportTemplatesQueryValidator : AbstractValidator<GetReportTemplatesQuery>
{
    public GetReportTemplatesQueryValidator()
    {
        RuleFor(x => x.Request).NotNull().WithErrorCode(ErrorCodes.RequestRequired);
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).WithErrorCode(ErrorCodes.PageNumberMustBePositive).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).WithErrorCode("Shared.MustBeBetween").When(x => x.Request is not null);
    }
}