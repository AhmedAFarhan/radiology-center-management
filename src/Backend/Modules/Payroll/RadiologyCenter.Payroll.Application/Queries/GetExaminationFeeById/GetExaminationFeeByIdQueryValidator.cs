using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFeeById;

public class GetExaminationFeeByIdQueryValidator : EntityIdQueryValidatorBase<GetExaminationFeeByIdQuery>
{
}