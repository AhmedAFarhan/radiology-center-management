using RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Events;

namespace RadiologyCenter.Examinations.Application.Events;

public static class ExaminationCheckedInEventHandler
{
    public static async Task HandleAsync(
        ExaminationCheckedInEvent e,
        IExaminationTypeDirectory examinationTypeDirectory,
        IPatientInfoResolver patientInfoResolver,
        INotificationService notificationService,
        CancellationToken ct)
    {
        var patient = await patientInfoResolver.ResolveAsync(e.PatientId, ct);
        var examType = await examinationTypeDirectory.GetByIdAsync(e.ExaminationTypeId, ct);

        var dto = new ExamCheckedInNotificationDto(
            e.ExaminationId.ToString(),
            e.PatientId.ToString(),
            patient?.FullName ?? $"Patient {e.PatientId}",
            patient?.PatientCode ?? "-",
            examType?.Name ?? "Examination",
            "CheckedIn",
            e.ScheduledAt,
            e.Priority.Name,
            e.Priority.Name,
            e.ClinicalIndication,
            e.RadiologistId?.ToString());

        await notificationService.BroadcastAsync("exams:checkedin", dto, ct);
    }
}
