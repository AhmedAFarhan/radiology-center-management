using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Catalog.Application.Localization;

namespace RadiologyCenter.Catalog.Application.Queries.GetExaminationTypeById;

public class GetExaminationTypeByIdQueryValidator : EntityIdQueryValidatorBase<GetExaminationTypeByIdQuery>
{
    public GetExaminationTypeByIdQueryValidator()
        : base(ErrorCodes.ExaminationTypeIdRequired)
    {
    }
}
