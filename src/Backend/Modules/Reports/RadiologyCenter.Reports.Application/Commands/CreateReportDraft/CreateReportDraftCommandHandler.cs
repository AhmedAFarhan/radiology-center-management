using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Commands.CreateReportDraft;

public static class CreateReportDraftCommandHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        CreateReportDraftCommand command,
        IReportRepository reportRepository,
        IReportsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (await reportRepository.HasReportByExaminationAsync(command.ExaminationId, ct))
            return Result.Failure<ReportDto>(Error.Conflict($"A report already exists for examination '{command.ExaminationId}'."));

        var report = RadiologyReport.Create(command.ExaminationId, command.PatientId, command.RadiologistId);

        await reportRepository.AddAsync(report, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(report.ToDto());
    }
}