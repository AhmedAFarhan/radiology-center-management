using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.CreateVisit;

public class CreateVisitCommandValidator : AbstractValidator<CreateVisitCommand>
{
    public CreateVisitCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.VisitedAt)
            .Must(v => v is null || v != default)
            .WithMessage("VisitedAt, when provided, cannot be the default value.");
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
