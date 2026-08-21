using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExamination;

public class CreateExaminationCommandValidator : AbstractValidator<CreateExaminationCommand>
{
    public CreateExaminationCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
        RuleFor(x => x.ClinicalIndication).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Priority).NotEmpty().IsEnumerationMember<ExaminationPriority, CreateExaminationCommand>("Priority");
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Discount).LessThanOrEqualTo(100).When(x => x.IsDiscountPercentage);
        RuleFor(x => x.Paid).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
