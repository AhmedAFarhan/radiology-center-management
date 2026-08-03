using FluentValidation;
using RadiologyCenter.Examinations.Application.Commands.Common;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationType;

public class UpdateExaminationTypeCommandValidator
    : ExaminationTypeValidatorBase<UpdateExaminationTypeCommand, UpdateExaminationTypeItemRequest>
{
    public UpdateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
    }
}
