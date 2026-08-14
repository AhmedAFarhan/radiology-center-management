using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Commands.CreateReportDraft;

public static class CreateReportDraftCommandHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        CreateReportDraftCommand command,
        IReportRepository reportRepository,
        IReportDirectory reportDirectory,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (await reportRepository.HasReportByExaminationAsync(command.ExaminationId, ct))
            return Result.Failure<ReportDto>(Error.Conflict($"A report already exists for examination '{command.ExaminationId}'."));

        if (!await reportDirectory.IsExaminationCompletedAsync(command.ExaminationId, ct))
            return Result.Failure<ReportDto>(Error.Validation("ExaminationNotCompleted", "A report draft can only be created for a completed examination."));

        var report = RadiologyReport.Create(command.ExaminationId, command.PatientId, command.RadiologistId);

        await reportRepository.AddAsync(report, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(report.ToDto());
    }
}