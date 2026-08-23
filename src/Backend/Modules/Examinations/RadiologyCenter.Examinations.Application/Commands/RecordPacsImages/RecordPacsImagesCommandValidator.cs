using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.RecordPacsImages;

public class RecordPacsImagesCommandValidator : AbstractValidator<RecordPacsImagesCommand>
{
    public RecordPacsImagesCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.StudyInstanceUID)
            .MaximumLength(64).WithErrorCode(SharedCodes.Shared.TextTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.StudyInstanceUID));
        RuleFor(x => x.AccessionNumber)
            .MaximumLength(64).WithErrorCode(SharedCodes.Shared.TextTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.AccessionNumber));
    }
}