using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Identity.Application.Localization;

namespace RadiologyCenter.Identity.Application.Queries.GetUserById;

public class GetUserByIdQueryValidator : EntityIdQueryValidatorBase<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator() : base(ErrorCodes.UserIdRequired)
    {
    }
}
