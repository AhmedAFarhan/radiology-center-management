using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignments;

public class GetAllowanceAssignmentsQueryValidator : AbstractValidator<GetAllowanceAssignmentsQuery>
{
    public GetAllowanceAssignmentsQueryValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).When(x => x.Request is not null);
    }
}