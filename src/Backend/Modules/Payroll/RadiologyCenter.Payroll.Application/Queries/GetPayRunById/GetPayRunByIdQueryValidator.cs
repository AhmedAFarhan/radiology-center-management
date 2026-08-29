using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Queries.GetPayRunById;

public class GetPayRunByIdQueryValidator : EntityIdQueryValidatorBase<GetPayRunByIdQuery>
{
    public GetPayRunByIdQueryValidator() : base(ErrorCodes.PayRunIdRequired)
    {
    }
}