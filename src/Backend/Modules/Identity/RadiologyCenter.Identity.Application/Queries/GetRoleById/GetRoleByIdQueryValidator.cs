using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Queries.GetRoleById;

public class GetRoleByIdQueryValidator : EntityIdQueryValidatorBase<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator() : base(ErrorCodes.RoleIdRequired)
    {
    }
}
