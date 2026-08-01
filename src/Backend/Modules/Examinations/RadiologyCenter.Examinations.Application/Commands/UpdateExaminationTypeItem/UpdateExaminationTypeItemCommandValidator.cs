using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationTypeItem;

public class UpdateExaminationTypeItemCommandValidator : AbstractValidator<UpdateExaminationTypeItemCommand>
{
    public UpdateExaminationTypeItemCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
        RuleFor(x => x.ExaminationTypeItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
