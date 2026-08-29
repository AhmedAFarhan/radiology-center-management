using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExamination;

public class CreateExaminationCommandValidator : AbstractValidator<CreateExaminationCommand>
{
    public CreateExaminationCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(ErrorCodes.PatientIdRequired);
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
        RuleFor(x => x.ClinicalIndication).NotEmpty().WithErrorCode(ErrorCodes.ClinicalIndicationRequired).MaximumLength(1000).WithErrorCode(ErrorCodes.ClinicalIndicationTooLong);
        RuleFor(x => x.Priority).NotEmpty().WithErrorCode(ErrorCodes.PriorityRequired).IsEnumerationMember<ExaminationPriority, CreateExaminationCommand>("Priority");
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.DiscountCannotBeNegative);
        RuleFor(x => x.Discount).LessThanOrEqualTo(100).When(x => x.IsDiscountPercentage).WithErrorCode(ErrorCodes.PercentageDiscountMax);
        RuleFor(x => x.Paid).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.PaidCannotBeNegative);
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(ErrorCodes.NotesTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
        RuleFor(x => x.Status)
            .Must(s => s == ExaminationStatus.Scheduled.Name || s == ExaminationStatus.CheckedIn.Name)
            .WithErrorCode(ErrorCodes.InvalidStatusTransition)
            .When(x => !string.IsNullOrWhiteSpace(x.Status));
    }
}
