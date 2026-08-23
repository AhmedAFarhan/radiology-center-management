using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.ApplyReportTemplate;

public class ApplyReportTemplateCommandValidator : AbstractValidator<ApplyReportTemplateCommand>
{
    public ApplyReportTemplateCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}