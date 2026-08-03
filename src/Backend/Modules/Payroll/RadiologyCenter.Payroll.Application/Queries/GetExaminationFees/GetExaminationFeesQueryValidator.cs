using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFees;

public class GetExaminationFeesQueryValidator : AbstractValidator<GetExaminationFeesQuery>
{
    public GetExaminationFeesQueryValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).When(x => x.Request is not null);
    }
}