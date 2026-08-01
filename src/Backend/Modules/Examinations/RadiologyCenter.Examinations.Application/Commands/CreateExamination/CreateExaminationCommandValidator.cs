using FluentValidation;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExamination;

public class CreateExaminationCommandValidator : AbstractValidator<CreateExaminationCommand>
{
    public CreateExaminationCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
        RuleFor(x => x.ReferringDoctor).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ClinicalIndication).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Priority).NotEmpty().Must(IsValidPriority)
            .WithMessage("Priority must be one of: Routine, Urgent, Stat.");
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Discount).LessThanOrEqualTo(100).When(x => x.IsDiscountPercentage);
        RuleFor(x => x.Paid).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }

    private static bool IsValidPriority(string priority) =>
        ExaminationPriority.GetAll<ExaminationPriority>().Any(p => p.Name.Equals(priority, StringComparison.OrdinalIgnoreCase));
}
