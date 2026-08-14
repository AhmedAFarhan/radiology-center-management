using FluentValidation;

namespace RadiologyCenter.Catalog.Application.Commands.DeleteExaminationType;

public class DeleteExaminationTypeCommandValidator : AbstractValidator<DeleteExaminationTypeCommand>
{
    public DeleteExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
    }
}
