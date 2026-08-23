using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeleteReportTemplate;

public class DeleteReportTemplateCommandValidator : AbstractValidator<DeleteReportTemplateCommand>
{
    public DeleteReportTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}