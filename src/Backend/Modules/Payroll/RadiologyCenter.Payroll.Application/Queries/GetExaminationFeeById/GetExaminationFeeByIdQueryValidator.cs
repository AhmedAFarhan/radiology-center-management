using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFeeById;

public class GetExaminationFeeByIdQueryValidator : EntityIdQueryValidatorBase<GetExaminationFeeByIdQuery>
{
    public GetExaminationFeeByIdQueryValidator() : base(ErrorCodes.ExaminationFeeIdRequired)
    {
    }
}