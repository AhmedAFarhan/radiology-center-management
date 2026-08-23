using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Inventory.Application.Queries.GetItemStock;

public class GetItemStockQueryValidator : AbstractValidator<GetItemStockQuery>
{
    public GetItemStockQueryValidator() => RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
}
