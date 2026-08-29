using RadiologyCenter.Inventory.Application.Localization;

namespace RadiologyCenter.Inventory.Application.Queries.GetSupplierById;

public class GetSupplierByIdQueryValidator : BuildingBlocks.Application.Validation.EntityIdQueryValidatorBase<GetSupplierByIdQuery>
{
    public GetSupplierByIdQueryValidator() : base(ErrorCodes.SupplierIdRequired)
    {
    }
}
