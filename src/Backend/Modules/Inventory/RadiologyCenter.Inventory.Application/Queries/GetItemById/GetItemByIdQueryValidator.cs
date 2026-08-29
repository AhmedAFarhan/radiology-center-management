using RadiologyCenter.Inventory.Application.Localization;

namespace RadiologyCenter.Inventory.Application.Queries.GetItemById;

public class GetItemByIdQueryValidator : BuildingBlocks.Application.Validation.EntityIdQueryValidatorBase<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator() : base(ErrorCodes.ItemIdRequired)
    {
    }
}
