using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.CheckInExamination;

public class CheckInExaminationCommandValidator : AbstractValidator<CheckInExaminationCommand>
{
    public CheckInExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
