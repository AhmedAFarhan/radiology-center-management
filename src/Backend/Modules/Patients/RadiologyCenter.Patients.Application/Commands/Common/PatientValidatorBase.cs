using FluentValidation;
using RadiologyCenter.Patients.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Patients.Domain.Enumerations;

namespace RadiologyCenter.Patients.Application.Commands.Common;

public abstract class PatientValidatorBase<T> : AbstractValidator<T> where T : IPatientFields
{
    protected PatientValidatorBase()
    {
        RuleFor(x => x.FullName).NotEmpty().WithErrorCode(ErrorCodes.NameRequired).MaximumLength(300).WithErrorCode(ErrorCodes.FullNameTooLong);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts(ErrorCodes.FullNameTwoParts);
        RuleFor(x => x.Gender).NotEmpty().WithErrorCode(ErrorCodes.GenderRequired).IsEnumerationMember<Gender, T>("Gender");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithErrorCode(ErrorCodes.PhoneNumberRequired).IsEgyptianPhoneNumber(ErrorCodes.PhoneNumberInvalid).MaximumLength(30).WithErrorCode(ErrorCodes.PhoneNumberTooLong);
        RuleFor(x => x).Must(x => x.DateOfBirth is not null || x.Age is not null)
            .WithErrorCode(ErrorCodes.DobOrAgeRequired);
        RuleFor(x => x.DateOfBirth).Must(d => d is null || d.Value.Date <= DateTime.UtcNow.Date)
            .WithErrorCode(ErrorCodes.DateOfBirthFuture);
        RuleFor(x => x.Age).Must(a => a is null || a is >= 0 and <= 150)
            .WithErrorCode(ErrorCodes.AgeOutOfRange);
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(ErrorCodes.EmailInvalid).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.BloodType).Must(IsValidBloodType)
            .WithErrorCode(ErrorCodes.BloodTypeInvalid)
            .When(x => !string.IsNullOrWhiteSpace(x.BloodType));
    }

    private static bool IsValidBloodType(string? bloodType) =>
        bloodType is null ||
        BloodType.GetAll<BloodType>().Any(b => b.Name.Equals(bloodType, StringComparison.OrdinalIgnoreCase));
}
