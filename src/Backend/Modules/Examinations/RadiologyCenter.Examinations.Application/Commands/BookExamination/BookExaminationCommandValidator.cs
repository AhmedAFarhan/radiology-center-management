using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.BookExamination;

public class BookExaminationCommandValidator : AbstractValidator<BookExaminationCommand>
{
    public BookExaminationCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(ErrorCodes.PatientIdRequired);
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
        RuleFor(x => x.ScheduledAt).NotEmpty().WithErrorCode(ErrorCodes.ScheduledAtRequired);
        RuleFor(x => x.Priority).NotEmpty().WithErrorCode(ErrorCodes.PriorityRequired)
            .IsEnumerationMember<ExaminationPriority, BookExaminationCommand>("Priority");
        RuleFor(x => x.ClinicalIndication).MaximumLength(1000).WithErrorCode(ErrorCodes.ClinicalIndicationTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.ClinicalIndication));
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(ErrorCodes.NotesTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
