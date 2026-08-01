using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;

public class RemoveExaminationItemCommandValidator : AbstractValidator<RemoveExaminationItemCommand>
{
    public RemoveExaminationItemCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.ExaminationItemId).NotEmpty();
    }
}
