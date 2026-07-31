using FluentValidation;

namespace RadiologyCenter.Inventory.Application.Queries.GetPurchaseOrderById;

public class GetPurchaseOrderByIdQueryValidator : AbstractValidator<GetPurchaseOrderByIdQuery>
{
    public GetPurchaseOrderByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
