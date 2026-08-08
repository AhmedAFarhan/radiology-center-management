using FluentValidation;

namespace RadiologyCenter.Reports.Application.Commands.CreateReportDraft;

public class CreateReportDraftCommandValidator : AbstractValidator<CreateReportDraftCommand>
{
    public CreateReportDraftCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.RadiologistId).NotEmpty();
    }
}