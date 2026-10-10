using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Application.Localization;
using RadiologyCenter.Reports.Application.DTOs;

namespace RadiologyCenter.Reports.Application.Commands.CreateReportDraft;

public static class CreateReportDraftCommandHandler
{
    public static async Task<Result<ReportDto>> HandleAsync(
        CreateReportDraftCommand command,
        IReportRepository reportRepository,
        IReportDirectory reportDirectory,
        IReportsUnitOfWork unitOfWork,
        ITransactionLock transactionLock,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        await transactionLock.AcquireAsync(transaction, new[] { "reports:writing" }, ct);
        if (await reportRepository.HasReportByExaminationAsync(command.ExaminationId, ct))
            return Result.Failure<ReportDto>(Error.Conflict(ErrorCodes.ReportAlreadyExists, "A report already exists for this examination."));

        if (!await reportDirectory.IsExaminationInStatusAsync(command.ExaminationId, ct))
            return Result.Failure<ReportDto>(Error.Validation(ErrorCodes.ExaminationNotCompleted, "A report draft can only be created for an examination that has been checked in, started or completed."));

        var identity = await reportDirectory.GetExaminationIdentityAsync(command.ExaminationId, ct);
        if (identity is null || identity.PatientId != command.PatientId)
            return Result.Failure<ReportDto>(Error.Validation("Reports.PatientMismatch", "The report patient must match the examination patient."));
        if (!await reportDirectory.IsActiveRadiologistAsync(command.RadiologistId, ct)
            || (identity.RadiologistId.HasValue && identity.RadiologistId != command.RadiologistId))
            return Result.Failure<ReportDto>(Error.Validation("Reports.RadiologistMismatch", "The report must use the active radiologist assigned to the examination."));

        var report = RadiologyReport.Create(command.ExaminationId, identity.PatientId, command.RadiologistId);

        await reportRepository.AddAsync(report, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Success(report.ToDto());
    }
}