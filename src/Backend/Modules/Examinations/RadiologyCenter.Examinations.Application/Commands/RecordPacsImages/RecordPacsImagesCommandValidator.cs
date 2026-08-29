using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.RecordPacsImages;

public class RecordPacsImagesCommandValidator : AbstractValidator<RecordPacsImagesCommand>
{
    public RecordPacsImagesCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.StudyInstanceUID)
            .MaximumLength(64).WithErrorCode(ErrorCodes.StudyInstanceUidTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.StudyInstanceUID));
        RuleFor(x => x.AccessionNumber)
            .MaximumLength(64).WithErrorCode(ErrorCodes.AccessionNumberTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.AccessionNumber));
    }
}
