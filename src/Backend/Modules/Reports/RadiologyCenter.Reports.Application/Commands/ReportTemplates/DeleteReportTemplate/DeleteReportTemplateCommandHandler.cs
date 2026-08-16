using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeleteReportTemplate;

public static class DeleteReportTemplateCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteReportTemplateCommand command,
        IReportTemplateRepository templateRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await templateRepository.GetByIdAsync(command.TemplateId, ct);
        if (template is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ReportTemplateNotFound, "ReportTemplate", command.TemplateId));

        if (template.IsSystem)
            return Result.Failure(Error.Conflict(ErrorCodes.SystemTemplateCannotDelete, "System templates cannot be deleted."));

        templateRepository.Remove(template);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}