using RadiologyCenter.Reports.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Commands.ReportTemplates.ActivateReportTemplate;

public static class ActivateReportTemplateCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateReportTemplateCommand command,
        IReportTemplateRepository templateRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var template = await templateRepository.GetByIdAsync(command.TemplateId, ct);
        if (template is null)
            return Result.Failure(Error.NotFound("ReportTemplate", command.TemplateId));

        template.Activate();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}