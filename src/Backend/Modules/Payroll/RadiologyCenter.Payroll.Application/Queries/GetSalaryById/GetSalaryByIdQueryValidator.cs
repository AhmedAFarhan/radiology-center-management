using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryById;

public class GetSalaryByIdQueryValidator : EntityIdQueryValidatorBase<GetSalaryByIdQuery>
{
    public GetSalaryByIdQueryValidator() : base(ErrorCodes.SalaryIdRequired)
    {
    }
}