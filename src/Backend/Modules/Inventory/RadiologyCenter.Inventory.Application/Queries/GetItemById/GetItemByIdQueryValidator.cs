using FluentValidation;

namespace RadiologyCenter.Inventory.Application.Queries.GetItemById;

public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
