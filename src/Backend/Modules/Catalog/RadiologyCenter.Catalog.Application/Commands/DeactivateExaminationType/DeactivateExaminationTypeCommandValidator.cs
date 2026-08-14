using FluentValidation;

namespace RadiologyCenter.Catalog.Application.Commands.DeactivateExaminationType;

public class DeactivateExaminationTypeCommandValidator : AbstractValidator<DeactivateExaminationTypeCommand>
{
    public DeactivateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
    }
}
