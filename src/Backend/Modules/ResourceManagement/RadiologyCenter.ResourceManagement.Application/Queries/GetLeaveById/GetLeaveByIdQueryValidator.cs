using FluentValidation;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetLeaveById;

public class GetLeaveByIdQueryValidator : AbstractValidator<GetLeaveByIdQuery>
{
    public GetLeaveByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
