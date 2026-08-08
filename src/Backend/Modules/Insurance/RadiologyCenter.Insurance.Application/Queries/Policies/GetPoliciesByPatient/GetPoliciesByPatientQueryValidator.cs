using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPoliciesByPatient;

public class GetPoliciesByPatientQueryValidator : AbstractValidator<GetPoliciesByPatientQuery>
{
    public GetPoliciesByPatientQueryValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
    }
}