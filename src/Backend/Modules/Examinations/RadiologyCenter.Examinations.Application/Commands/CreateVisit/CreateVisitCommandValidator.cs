using FluentValidation;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.CreateVisit;

public class CreateVisitCommandValidator : AbstractValidator<CreateVisitCommand>
{
    public CreateVisitCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();

        RuleFor(x => x.Examinations)
            .NotEmpty()
            .WithMessage("A visit must include at least one examination.");

        RuleForEach(x => x.Examinations).ChildRules(examination =>
        {
            examination.RuleFor(e => e.ExaminationTypeId).NotEmpty();
            examination.RuleFor(e => e.ReferringDoctor).NotEmpty().MaximumLength(200);
            examination.RuleFor(e => e.ClinicalIndication).NotEmpty().MaximumLength(1000);
            examination.RuleFor(e => e.Priority).NotEmpty().Must(IsValidPriority)
                .WithMessage("Priority must be one of: Routine, Urgent, Stat.");
            examination.RuleFor(e => e.Notes).MaximumLength(500).When(e => !string.IsNullOrWhiteSpace(e.Notes));

            examination.RuleFor(e => e.Items)
                .Must(items => items is null || items.Select(i => i.ItemId).Distinct().Count() == items.Count)
                .WithMessage("An item can only be added once per examination.")
                .When(e => e.Items is not null);

            examination.RuleForEach(e => e.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ItemId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0);
                item.RuleFor(i => i.Notes).MaximumLength(500).When(i => !string.IsNullOrWhiteSpace(i.Notes));
            });
        });

        RuleFor(x => x.VisitedAt)
            .Must(v => v is null || v != default)
            .WithMessage("VisitedAt, when provided, cannot be the default value.");

        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }

    private static bool IsValidPriority(string priority) =>
        ExaminationPriority.GetAll<ExaminationPriority>().Any(p => p.Name.Equals(priority, StringComparison.OrdinalIgnoreCase));
}
