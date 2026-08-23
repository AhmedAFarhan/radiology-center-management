using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.ActivateReportTemplate;

public class ActivateReportTemplateCommandValidator : AbstractValidator<ActivateReportTemplateCommand>
{
    public ActivateReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}