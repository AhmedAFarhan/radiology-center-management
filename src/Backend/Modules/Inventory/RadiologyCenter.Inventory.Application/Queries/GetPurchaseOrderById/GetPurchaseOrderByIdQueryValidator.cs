using RadiologyCenter.Inventory.Application.Localization;

namespace RadiologyCenter.Inventory.Application.Queries.GetPurchaseOrderById;

public class GetPurchaseOrderByIdQueryValidator : BuildingBlocks.Application.Validation.EntityIdQueryValidatorBase<GetPurchaseOrderByIdQuery>
{
    public GetPurchaseOrderByIdQueryValidator() : base(ErrorCodes.PurchaseOrderIdRequired)
    {
    }
}
