using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetLeaveById;

public class GetLeaveByIdQueryValidator : EntityIdQueryValidatorBase<GetLeaveByIdQuery>
{
    public GetLeaveByIdQueryValidator() : base(ErrorCodes.LeaveIdRequired)
    {
    }
}
