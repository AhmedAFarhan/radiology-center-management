using FluentValidation;

namespace RadiologyCenter.Reports.Application.Queries.GetReportTemplates;

public class GetReportTemplatesQueryValidator : AbstractValidator<GetReportTemplatesQuery>
{
    public GetReportTemplatesQueryValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).When(x => x.Request is not null);
    }
}