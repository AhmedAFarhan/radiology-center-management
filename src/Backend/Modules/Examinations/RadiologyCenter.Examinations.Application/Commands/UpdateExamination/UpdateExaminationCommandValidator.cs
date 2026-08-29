using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public class UpdateExaminationCommandValidator : AbstractValidator<UpdateExaminationCommand>
{
    public UpdateExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(ErrorCodes.PatientIdRequired).When(x => x.PatientId.HasValue);
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired).When(x => x.ExaminationTypeId.HasValue);
        RuleFor(x => x.Status)
            .Must(s => s is null || s == ExaminationStatus.Scheduled.Name || s == ExaminationStatus.CheckedIn.Name)
            .WithErrorCode(ErrorCodes.InvalidStatusTransition);
        RuleFor(x => x.ClinicalIndication).NotEmpty().WithErrorCode(ErrorCodes.ClinicalIndicationRequired).MaximumLength(1000).WithErrorCode(ErrorCodes.ClinicalIndicationTooLong);
        RuleFor(x => x.Priority).NotEmpty().WithErrorCode(ErrorCodes.PriorityRequired).IsEnumerationMember<ExaminationPriority, UpdateExaminationCommand>("Priority");
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(ErrorCodes.NotesTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.DiscountCannotBeNegative).When(x => x.Discount.HasValue);
        RuleFor(x => x.Discount).LessThanOrEqualTo(100)
            .When(x => x.Discount.HasValue && x.IsDiscountPercentage == true)
            .WithErrorCode(ErrorCodes.PercentageDiscountMax);
        RuleFor(x => x.Paid).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.PaidCannotBeNegative).When(x => x.Paid.HasValue);

        RuleFor(x => x.Items)
            .Must(items => items is null || items.Select(i => i.ItemId).Distinct().Count() == items.Count)
            .WithErrorCode(ErrorCodes.DuplicateItem)
            .When(x => x.Items is not null);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId).NotEmpty().WithErrorCode(ErrorCodes.ItemIdRequired);
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithErrorCode(ErrorCodes.QuantityMustBePositive);
            item.RuleFor(i => i.Notes).MaximumLength(500).WithErrorCode(ErrorCodes.NotesTooLong).When(i => !string.IsNullOrWhiteSpace(i.Notes));
        });
    }
}
