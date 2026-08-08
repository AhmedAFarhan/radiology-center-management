using RadiologyCenter.Reports.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.DeactivateReportTemplate;

public static class DeactivateReportTemplateCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateReportTemplateCommand command,
        IReportTemplateRepository templateRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await templateRepository.GetByIdAsync(command.TemplateId, ct);
        if (template is null)
            return Result.Failure(Error.NotFound("ReportTemplate", command.TemplateId));

        template.Deactivate();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}