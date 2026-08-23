using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class ReferralDoctorValidatorBase<T> : AbstractValidator<T> where T : IReferralDoctorFields
{
    protected ReferralDoctorValidatorBase()
    {
        RuleFor(x => x.FullName).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(300).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts();
        RuleFor(x => x.Phone).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Specialization).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.Hospital).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Hospital));
    }
}
