using FluentValidation;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShiftById;

public class GetWorkShiftByIdQueryValidator : AbstractValidator<GetWorkShiftByIdQuery>
{
    public GetWorkShiftByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
