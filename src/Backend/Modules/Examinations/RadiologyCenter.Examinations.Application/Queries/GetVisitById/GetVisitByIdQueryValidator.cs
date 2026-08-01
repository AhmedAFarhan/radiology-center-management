using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Queries.GetVisitById;

public class GetVisitByIdQueryValidator : AbstractValidator<GetVisitByIdQuery>
{
    public GetVisitByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
