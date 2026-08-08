namespace RadiologyCenter.Reports.Application.Commands.CreateReportDraft;

public record CreateReportDraftCommand(
    Guid ExaminationId,
    Guid PatientId,
    Guid RadiologistId) : ICommand;