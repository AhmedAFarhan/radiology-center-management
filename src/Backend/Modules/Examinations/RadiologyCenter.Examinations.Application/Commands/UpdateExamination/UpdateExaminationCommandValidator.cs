using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public class UpdateExaminationCommandValidator : AbstractValidator<UpdateExaminationCommand>
{
    public UpdateExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.ReferringDoctor).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ClinicalIndication).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Priority).NotEmpty().IsEnumerationMember<ExaminationPriority, UpdateExaminationCommand>("Priority");
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).When(x => x.Discount.HasValue);
        RuleFor(x => x.Discount).LessThanOrEqualTo(100)
            .When(x => x.Discount.HasValue && x.IsDiscountPercentage == true)
            .WithMessage("Percentage discount cannot exceed 100.");
        RuleFor(x => x.Paid).GreaterThanOrEqualTo(0).When(x => x.Paid.HasValue);

        RuleFor(x => x.Items)
            .Must(items => items is null || items.Select(i => i.ItemId).Distinct().Count() == items.Count)
            .WithMessage("An item can only be added once per examination.")
            .When(x => x.Items is not null);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.Notes).MaximumLength(500).When(i => !string.IsNullOrWhiteSpace(i.Notes));
        });
    }
}
