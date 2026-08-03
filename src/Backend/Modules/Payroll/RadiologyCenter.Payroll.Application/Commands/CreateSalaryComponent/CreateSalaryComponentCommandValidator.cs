using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.CreateSalaryComponent;

public class CreateSalaryComponentCommandValidator : AbstractValidator<CreateSalaryComponentCommand>
{
    public CreateSalaryComponentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Kind).NotEmpty().IsEnumerationMember<ComponentKind, CreateSalaryComponentCommand>("Kind");
        RuleFor(x => x.DefaultValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Frequency)
            .Must(BeValidFrequency)
            .WithMessage($"Frequency must be one of: {string.Join(", ", Frequency.GetAll<Frequency>().Select(f => f.Name))}.");
    }

    private static bool BeValidFrequency(string? frequency) =>
        string.IsNullOrWhiteSpace(frequency) || Frequency.GetAll<Frequency>().Any(f => f.Name.Equals(frequency, StringComparison.OrdinalIgnoreCase));
}