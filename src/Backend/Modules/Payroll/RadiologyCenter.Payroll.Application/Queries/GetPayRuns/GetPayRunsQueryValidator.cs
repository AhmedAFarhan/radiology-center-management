using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Queries.GetPayRuns;

public class GetPayRunsQueryValidator : AbstractValidator<GetPayRunsQuery>
{
    public GetPayRunsQueryValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).When(x => x.Request is not null);
    }
}