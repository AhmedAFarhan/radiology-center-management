using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateReferralDoctor;

public class CreateReferralDoctorCommandValidator : AbstractValidator<CreateReferralDoctorCommand>
{
    public CreateReferralDoctorCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.FullName).Must(ContainsAtLeastTwoTokens)
            .WithMessage("Full name must contain at least a first name and a last name.");
        RuleFor(x => x.Phone).NotEmpty().IsEgyptianPhoneNumber().MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Specialization).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.Hospital).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Hospital));
    }

    private static bool ContainsAtLeastTwoTokens(string fullName)
    {
        var parts = fullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts is { Length: >= 2 };
    }
}
