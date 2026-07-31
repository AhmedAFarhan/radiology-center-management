using FluentValidation;
using RadiologyCenter.Patients.Domain.Enumerations;

namespace RadiologyCenter.Patients.Application.Commands.CreatePatient;

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.FullName).Must(ContainsAtLeastTwoTokens)
            .WithMessage("Full name must contain at least a first name and a last name.");
        RuleFor(x => x.Gender).NotEmpty().Must(IsValidGender)
            .WithMessage("Gender must be one of: Male, Female, Other.");
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
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

    private static bool ContainsAtLeastTwoTokens(string fullName)
    {
        var parts = fullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts is { Length: >= 2 };
    }

    private static bool IsValidGender(string gender) =>
        Gender.GetAll<Gender>().Any(g => g.Name.Equals(gender, StringComparison.OrdinalIgnoreCase));

    private static bool IsValidBloodType(string? bloodType) =>
        bloodType is null ||
        BloodType.GetAll<BloodType>().Any(b => b.Name.Equals(bloodType, StringComparison.OrdinalIgnoreCase));
}
