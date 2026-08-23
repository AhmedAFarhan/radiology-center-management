using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.CreateReportDraft;

public class CreateReportDraftCommandValidator : AbstractValidator<CreateReportDraftCommand>
{
    public CreateReportDraftCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.RadiologistId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}