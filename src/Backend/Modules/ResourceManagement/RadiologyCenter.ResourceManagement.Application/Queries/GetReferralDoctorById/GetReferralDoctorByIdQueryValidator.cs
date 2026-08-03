using FluentValidation;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetReferralDoctorById;

public class GetReferralDoctorByIdQueryValidator : AbstractValidator<GetReferralDoctorByIdQuery>
{
    public GetReferralDoctorByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
