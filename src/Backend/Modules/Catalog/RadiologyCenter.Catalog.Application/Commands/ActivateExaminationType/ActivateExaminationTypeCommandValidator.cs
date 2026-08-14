using FluentValidation;

namespace RadiologyCenter.Catalog.Application.Commands.ActivateExaminationType;

public class ActivateExaminationTypeCommandValidator : AbstractValidator<ActivateExaminationTypeCommand>
{
    public ActivateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
    }
}
