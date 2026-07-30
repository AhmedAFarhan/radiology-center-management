using FluentValidation;

namespace RadiologyCenter.Idnetity.Application.Queries.GetUserById;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
