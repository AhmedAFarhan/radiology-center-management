using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.DeactivateExaminationType;

public class DeactivateExaminationTypeCommandValidator : AbstractValidator<DeactivateExaminationTypeCommand>
{
    public DeactivateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
    }
}
