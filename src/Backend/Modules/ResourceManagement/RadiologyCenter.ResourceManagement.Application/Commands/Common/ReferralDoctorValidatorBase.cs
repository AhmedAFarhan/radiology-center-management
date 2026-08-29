using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class ReferralDoctorValidatorBase<T> : AbstractValidator<T> where T : IReferralDoctorFields
{
    protected ReferralDoctorValidatorBase()
    {
        RuleFor(x => x.FullName).NotEmpty().WithErrorCode(ErrorCodes.FullNameRequired).MaximumLength(300).WithErrorCode(ErrorCodes.FullNameTooLong);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts();
        RuleFor(x => x.Phone).NotEmpty().WithErrorCode(ErrorCodes.PhoneRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(ErrorCodes.PhoneTooLong);
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(ErrorCodes.InvalidEmail).MaximumLength(200).WithErrorCode(ErrorCodes.EmailTooLong).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Specialization).MaximumLength(200).WithErrorCode(ErrorCodes.SpecializationTooLong).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.Hospital).MaximumLength(200).WithErrorCode(ErrorCodes.HospitalTooLong).When(x => !string.IsNullOrWhiteSpace(x.Hospital));
    }
}
