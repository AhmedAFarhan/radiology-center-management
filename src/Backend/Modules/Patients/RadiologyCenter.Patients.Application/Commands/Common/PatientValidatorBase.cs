using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Patients.Domain.Enumerations;

namespace RadiologyCenter.Patients.Application.Commands.Common;

public abstract class PatientValidatorBase<T> : AbstractValidator<T> where T : IPatientFields
{
    protected PatientValidatorBase()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts();
        RuleFor(x => x.Gender).NotEmpty().IsEnumerationMember<Gender, T>("Gender");
        RuleFor(x => x.PhoneNumber).NotEmpty().IsEgyptianPhoneNumber().MaximumLength(30);
        RuleFor(x => x).Must(x => x.DateOfBirth is not null || x.Age is not null)
            .WithMessage("Either date of birth or age must be provided.");
        RuleFor(x => x.DateOfBirth).Must(d => d is null || d.Value.Date <= DateTime.UtcNow.Date)
            .WithMessage("Date of birth cannot be in the future.");
        RuleFor(x => x.Age).Must(a => a is null || a is >= 0 and <= 150)
            .WithMessage("Age must be between 0 and 150.");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.BloodType).Must(IsValidBloodType)
            .WithMessage("Blood type is invalid.")
            .When(x => !string.IsNullOrWhiteSpace(x.BloodType));
    }

    private static bool IsValidBloodType(string? bloodType) =>
        bloodType is null ||
        BloodType.GetAll<BloodType>().Any(b => b.Name.Equals(bloodType, StringComparison.OrdinalIgnoreCase));
}
