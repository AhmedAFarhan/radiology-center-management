using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Commands.CreateReportDraft;

public class CreateReportDraftCommandValidator : AbstractValidator<CreateReportDraftCommand>
{
    public CreateReportDraftCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(ErrorCodes.PatientIdRequired);
        RuleFor(x => x.RadiologistId).NotEmpty().WithErrorCode(ErrorCodes.RadiologistIdRequired);
    }
}