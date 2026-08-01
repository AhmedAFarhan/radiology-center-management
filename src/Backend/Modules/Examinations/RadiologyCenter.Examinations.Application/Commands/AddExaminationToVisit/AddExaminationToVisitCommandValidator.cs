using FluentValidation;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationToVisit;

public class AddExaminationToVisitCommandValidator : AbstractValidator<AddExaminationToVisitCommand>
{
    public AddExaminationToVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
        RuleFor(x => x.ReferringDoctor).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ClinicalIndication).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Priority).NotEmpty().Must(IsValidPriority)
            .WithMessage("Priority must be one of: Routine, Urgent, Stat.");
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }

    private static bool IsValidPriority(string priority) =>
        ExaminationPriority.GetAll<ExaminationPriority>().Any(p => p.Name.Equals(priority, StringComparison.OrdinalIgnoreCase));
}
