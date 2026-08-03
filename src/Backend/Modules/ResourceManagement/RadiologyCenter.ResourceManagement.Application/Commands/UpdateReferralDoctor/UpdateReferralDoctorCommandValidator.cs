using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateReferralDoctor;

public class UpdateReferralDoctorCommandValidator : AbstractValidator<UpdateReferralDoctorCommand>
{
    public UpdateReferralDoctorCommandValidator()
    {
        RuleFor(x => x.ReferralDoctorId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().IsEgyptianPhoneNumber().MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Specialization).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.Hospital).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Hospital));
    }
}
