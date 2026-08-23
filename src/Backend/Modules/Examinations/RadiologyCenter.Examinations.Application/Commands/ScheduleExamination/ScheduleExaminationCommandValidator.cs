using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public class ScheduleExaminationCommandValidator : AbstractValidator<ScheduleExaminationCommand>
{
    public ScheduleExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ScheduledAt).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.ScheduledAt)
            .Must(s => s >= DateTime.UtcNow.AddMinutes(-1))
            .WithErrorCode(ErrorCodes.ScheduledTimePast);
    }
}
