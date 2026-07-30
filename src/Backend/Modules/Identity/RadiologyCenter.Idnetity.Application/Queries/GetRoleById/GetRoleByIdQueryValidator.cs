using FluentValidation;

namespace RadiologyCenter.Idnetity.Application.Queries.GetRoleById;

public class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
