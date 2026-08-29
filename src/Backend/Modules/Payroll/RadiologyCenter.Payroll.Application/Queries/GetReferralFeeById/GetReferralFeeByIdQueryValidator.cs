using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Queries.GetReferralFeeById;

public class GetReferralFeeByIdQueryValidator : EntityIdQueryValidatorBase<GetReferralFeeByIdQuery>
{
    public GetReferralFeeByIdQueryValidator() : base(ErrorCodes.ReferralFeeIdRequired)
    {
    }
}