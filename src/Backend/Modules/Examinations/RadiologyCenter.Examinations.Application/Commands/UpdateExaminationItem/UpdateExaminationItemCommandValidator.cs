using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationItem;

public class UpdateExaminationItemCommandValidator : AbstractValidator<UpdateExaminationItemCommand>
{
    public UpdateExaminationItemCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.ExaminationItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
