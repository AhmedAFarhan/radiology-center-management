using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.CompleteExamination;

public class CompleteExaminationCommandValidator : AbstractValidator<CompleteExaminationCommand>
{
    public CompleteExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
