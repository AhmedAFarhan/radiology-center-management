using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeactivateReportTemplate;

public class DeactivateReportTemplateCommandValidator : AbstractValidator<DeactivateReportTemplateCommand>
{
    public DeactivateReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}