using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetEquipmentById;

public class GetEquipmentByIdQueryValidator : EntityIdQueryValidatorBase<GetEquipmentByIdQuery>
{
    public GetEquipmentByIdQueryValidator() : base(ErrorCodes.EquipmentIdRequired)
    {
    }
}
