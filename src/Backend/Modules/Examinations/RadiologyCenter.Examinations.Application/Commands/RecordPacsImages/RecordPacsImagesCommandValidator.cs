using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.RecordPacsImages;

public class RecordPacsImagesCommandValidator : AbstractValidator<RecordPacsImagesCommand>
{
    public RecordPacsImagesCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.StudyInstanceUID)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.StudyInstanceUID));
        RuleFor(x => x.AccessionNumber)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.AccessionNumber));
    }
}