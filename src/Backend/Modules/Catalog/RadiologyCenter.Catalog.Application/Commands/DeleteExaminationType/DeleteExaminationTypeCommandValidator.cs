using FluentValidation;
using RadiologyCenter.Catalog.Application.Localization;

namespace RadiologyCenter.Catalog.Application.Commands.DeleteExaminationType;

public class DeleteExaminationTypeCommandValidator : AbstractValidator<DeleteExaminationTypeCommand>
{
    public DeleteExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
    }
}
