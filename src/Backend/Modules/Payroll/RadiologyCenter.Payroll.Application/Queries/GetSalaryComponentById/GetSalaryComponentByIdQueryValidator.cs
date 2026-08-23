using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponentById;

public class GetSalaryComponentByIdQueryValidator : EntityIdQueryValidatorBase<GetSalaryComponentByIdQuery>
{
}