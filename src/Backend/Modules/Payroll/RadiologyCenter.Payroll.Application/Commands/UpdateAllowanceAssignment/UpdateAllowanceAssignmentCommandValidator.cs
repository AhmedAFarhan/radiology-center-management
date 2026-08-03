using FluentValidation;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateAllowanceAssignment;

public class UpdateAllowanceAssignmentCommandValidator : AbstractValidator<UpdateAllowanceAssignmentCommand>
{
    public UpdateAllowanceAssignmentCommandValidator()
    {
        RuleFor(x => x.AllowanceAssignmentId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EffectiveDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.EffectiveDate)
            .When(x => x.EndDate.HasValue);
        RuleFor(x => x.Frequency)
            .Must(BeValidFrequency)
            .WithMessage($"Frequency must be one of: {string.Join(", ", Frequency.GetAll<Frequency>().Select(f => f.Name))}.");
    }

    private static bool BeValidFrequency(string? frequency) =>
        string.IsNullOrWhiteSpace(frequency) || Frequency.GetAll<Frequency>().Any(f => f.Name.Equals(frequency, StringComparison.OrdinalIgnoreCase));
}