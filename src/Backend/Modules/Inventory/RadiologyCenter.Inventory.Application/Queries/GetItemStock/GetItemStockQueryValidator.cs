using FluentValidation;

namespace RadiologyCenter.Inventory.Application.Queries.GetItemStock;

public class GetItemStockQueryValidator : AbstractValidator<GetItemStockQuery>
{
    public GetItemStockQueryValidator() => RuleFor(x => x.ItemId).NotEmpty();
}
