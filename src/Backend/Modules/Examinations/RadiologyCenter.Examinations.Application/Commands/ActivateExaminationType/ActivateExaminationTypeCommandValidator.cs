using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.ActivateExaminationType;

public class ActivateExaminationTypeCommandValidator : AbstractValidator<ActivateExaminationTypeCommand>
{
    public ActivateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
    }
}
