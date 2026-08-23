using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPoliciesByPatient;

public class GetPoliciesByPatientQueryValidator : AbstractValidator<GetPoliciesByPatientQuery>
{
    public GetPoliciesByPatientQueryValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}