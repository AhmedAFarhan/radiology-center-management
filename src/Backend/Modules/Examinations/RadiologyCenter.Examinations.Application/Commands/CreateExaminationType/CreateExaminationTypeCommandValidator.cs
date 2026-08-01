using FluentValidation;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExaminationType;

public class CreateExaminationTypeCommandValidator : AbstractValidator<CreateExaminationTypeCommand>
{
    public CreateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Modality).NotEmpty().Must(IsValidModality)
            .WithMessage("Modality must be one of: XRay, CT, MRI, Ultrasound, Mammography, Fluoroscopy, DEXA, Other.");
        RuleFor(x => x.BodyPart).NotEmpty().MaximumLength(200);
        RuleFor(x => x.StandardDurationMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);

        RuleFor(x => x.Items)
            .Must(items => items is null || items.Select(i => i.ItemId).Distinct().Count() == items.Count)
            .WithMessage("An item can only be added once per examination type.")
            .When(x => x.Items is not null);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.Notes).MaximumLength(500).When(i => !string.IsNullOrWhiteSpace(i.Notes));
        });
    }

    private static bool IsValidModality(string modality) =>
        Modality.GetAll<Modality>().Any(m => m.Name.Equals(modality, StringComparison.OrdinalIgnoreCase));
}
