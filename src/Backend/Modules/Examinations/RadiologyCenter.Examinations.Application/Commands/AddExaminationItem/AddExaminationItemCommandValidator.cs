using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;

public class AddExaminationItemCommandValidator : AbstractValidator<AddExaminationItemCommand>
{
    public AddExaminationItemCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
