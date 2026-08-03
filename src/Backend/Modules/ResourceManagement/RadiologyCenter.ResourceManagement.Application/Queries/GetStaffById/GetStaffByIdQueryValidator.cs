using FluentValidation;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetStaffById;

public class GetStaffByIdQueryValidator : AbstractValidator<GetStaffByIdQuery>
{
    public GetStaffByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
