using FluentValidation;

namespace RadiologyCenter.Identity.Application.Queries.GetRoleById;

public class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
