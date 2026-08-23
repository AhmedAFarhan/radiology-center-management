using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.CancelExamination;

public class CancelExaminationCommandValidator : AbstractValidator<CancelExaminationCommand>
{
    public CancelExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Reason).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}
