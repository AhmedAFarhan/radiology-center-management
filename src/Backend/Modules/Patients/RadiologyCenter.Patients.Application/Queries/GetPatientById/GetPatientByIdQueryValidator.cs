using FluentValidation;

namespace RadiologyCenter.Patients.Application.Queries.GetPatientById;

public class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
