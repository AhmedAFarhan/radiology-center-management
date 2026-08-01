using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminations;

public class GetExaminationsQueryValidator : AbstractValidator<GetExaminationsQuery>
{
    public GetExaminationsQueryValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).When(x => x.Request is not null);
    }
}
