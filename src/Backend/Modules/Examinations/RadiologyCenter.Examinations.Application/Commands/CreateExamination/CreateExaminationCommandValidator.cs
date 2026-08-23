using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExamination;

public class CreateExaminationCommandValidator : AbstractValidator<CreateExaminationCommand>
{
    public CreateExaminationCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ClinicalIndication).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(1000).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Priority).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<ExaminationPriority, CreateExaminationCommand>("Priority");
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Discount).LessThanOrEqualTo(100).When(x => x.IsDiscountPercentage);
        RuleFor(x => x.Paid).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
        RuleFor(x => x.Status)
            .Must(s => s == ExaminationStatus.Scheduled.Name || s == ExaminationStatus.CheckedIn.Name)
            .WithErrorCode(ErrorCodes.InvalidStatusTransition)
            .When(x => !string.IsNullOrWhiteSpace(x.Status));
    }
}
