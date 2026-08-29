using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShiftById;

public class GetWorkShiftByIdQueryValidator : EntityIdQueryValidatorBase<GetWorkShiftByIdQuery>
{
    public GetWorkShiftByIdQueryValidator() : base(ErrorCodes.WorkShiftIdRequired)
    {
    }
}
