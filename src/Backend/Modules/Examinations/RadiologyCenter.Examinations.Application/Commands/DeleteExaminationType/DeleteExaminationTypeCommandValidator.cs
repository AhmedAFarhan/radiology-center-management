using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.DeleteExaminationType;

public class DeleteExaminationTypeCommandValidator : AbstractValidator<DeleteExaminationTypeCommand>
{
    public DeleteExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
    }
}
