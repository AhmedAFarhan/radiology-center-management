using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetStaffById;

public class GetStaffByIdQueryValidator : EntityIdQueryValidatorBase<GetStaffByIdQuery>
{
    public GetStaffByIdQueryValidator() : base(ErrorCodes.StaffIdRequired)
    {
    }
}
