using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.CancelVisit;

public class CancelVisitCommandValidator : AbstractValidator<CancelVisitCommand>
{
    public CancelVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}
