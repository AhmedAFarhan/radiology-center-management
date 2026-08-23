using FluentValidation;
using RadiologyCenter.Patients.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Patients.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Patients.Application.Commands.Common;

public abstract class PatientValidatorBase<T> : AbstractValidator<T> where T : IPatientFields
{
    protected PatientValidatorBase()
    {
        RuleFor(x => x.FullName).NotEmpty().WithErrorCode(ErrorCodes.NameRequired).MaximumLength(300).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts();
        RuleFor(x => x.Gender).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<Gender, T>("Gender");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x).Must(x => x.DateOfBirth is not null || x.Age is not null)
            .WithErrorCode(ErrorCodes.DobOrAgeRequired);
        RuleFor(x => x.DateOfBirth).Must(d => d is null || d.Value.Date <= DateTime.UtcNow.Date)
            .WithErrorCode(ErrorCodes.DateOfBirthFuture);
        RuleFor(x => x.Age).Must(a => a is null || a is >= 0 and <= 150)
            .WithErrorCode(ErrorCodes.AgeOutOfRange);
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(SharedCodes.Shared.InvalidEmail).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.BloodType).Must(IsValidBloodType)
            .WithErrorCode(ErrorCodes.BloodTypeInvalid)
            .When(x => !string.IsNullOrWhiteSpace(x.BloodType));
    }

    private static bool IsValidBloodType(string? bloodType) =>
        bloodType is null ||
        BloodType.GetAll<BloodType>().Any(b => b.Name.Equals(bloodType, StringComparison.OrdinalIgnoreCase));
}
