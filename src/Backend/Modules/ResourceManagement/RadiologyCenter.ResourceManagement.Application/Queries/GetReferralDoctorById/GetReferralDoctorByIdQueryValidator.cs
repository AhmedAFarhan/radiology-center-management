using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetReferralDoctorById;

public class GetReferralDoctorByIdQueryValidator : EntityIdQueryValidatorBase<GetReferralDoctorByIdQuery>
{
    public GetReferralDoctorByIdQueryValidator() : base(ErrorCodes.ReferralDoctorIdRequired)
    {
    }
}
