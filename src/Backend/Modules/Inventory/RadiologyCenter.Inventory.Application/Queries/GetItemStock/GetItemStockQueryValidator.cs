using FluentValidation;
using RadiologyCenter.Inventory.Application.Localization;

namespace RadiologyCenter.Inventory.Application.Queries.GetItemStock;

public class GetItemStockQueryValidator : AbstractValidator<GetItemStockQuery>
{
    public GetItemStockQueryValidator() => RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(ErrorCodes.ItemIdRequired);
}
