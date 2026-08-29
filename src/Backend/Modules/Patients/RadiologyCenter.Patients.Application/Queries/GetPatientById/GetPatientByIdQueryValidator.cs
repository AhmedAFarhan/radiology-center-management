using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Patients.Application.Localization;

namespace RadiologyCenter.Patients.Application.Queries.GetPatientById;

public class GetPatientByIdQueryValidator : EntityIdQueryValidatorBase<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator() : base(ErrorCodes.PatientIdRequired)
    {
    }
}
