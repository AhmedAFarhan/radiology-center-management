using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class ReferralDoctorValidatorBase<T> : AbstractValidator<T> where T : IReferralDoctorFields
{
    protected ReferralDoctorValidatorBase()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts();
        RuleFor(x => x.Phone).NotEmpty().IsEgyptianPhoneNumber().MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Specialization).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.Hospital).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Hospital));
    }
}
