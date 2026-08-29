using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponentById;

public class GetSalaryComponentByIdQueryValidator : EntityIdQueryValidatorBase<GetSalaryComponentByIdQuery>
{
    public GetSalaryComponentByIdQueryValidator() : base(ErrorCodes.SalaryComponentIdRequired)
    {
    }
}