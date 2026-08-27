using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Examinations.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.BookExamination;

public class BookExaminationCommandValidator : AbstractValidator<BookExaminationCommand>
{
    public BookExaminationCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ScheduledAt).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.Priority).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired)
            .IsEnumerationMember<ExaminationPriority, BookExaminationCommand>("Priority");
        RuleFor(x => x.ClinicalIndication).MaximumLength(1000).WithErrorCode(SharedCodes.Shared.TextTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.ClinicalIndication));
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
