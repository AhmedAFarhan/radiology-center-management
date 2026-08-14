using FluentValidation;
using RadiologyCenter.Catalog.Application.Commands.Common;

namespace RadiologyCenter.Catalog.Application.Commands.UpdateExaminationType;

public class UpdateExaminationTypeCommandValidator
    : ExaminationTypeValidatorBase<UpdateExaminationTypeCommand>
{
    public UpdateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
    }
}