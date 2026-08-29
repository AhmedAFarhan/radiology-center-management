using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignmentById;

public class GetAllowanceAssignmentByIdQueryValidator : EntityIdQueryValidatorBase<GetAllowanceAssignmentByIdQuery>
{
    public GetAllowanceAssignmentByIdQueryValidator() : base(ErrorCodes.AllowanceAssignmentIdRequired)
    {
    }
}