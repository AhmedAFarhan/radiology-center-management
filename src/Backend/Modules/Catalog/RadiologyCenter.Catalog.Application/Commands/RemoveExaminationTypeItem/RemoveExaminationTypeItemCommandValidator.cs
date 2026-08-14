using FluentValidation;

namespace RadiologyCenter.Catalog.Application.Commands.RemoveExaminationTypeItem;

public class RemoveExaminationTypeItemCommandValidator : AbstractValidator<RemoveExaminationTypeItemCommand>
{
    public RemoveExaminationTypeItemCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
        RuleFor(x => x.ExaminationTypeItemId).NotEmpty();
    }
}
